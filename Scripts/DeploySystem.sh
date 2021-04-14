cd $(dirname "$0")
cd .. 

sh Scripts/DestroySystem.sh

kubectl create namespace development
kubectl config set-context --current --namespace=development

helm dep update ./SystemCharts/ClusterResources

helm install cluster-resources ./SystemCharts/ClusterResources --namespace development --wait

declare -a kafka_topics=("qa-updated" "intents-updated")

for topic in "${kafka_topics[@]}"
do
   kubectl exec --stdin --tty cluster-resources-kafka-0 -- /opt/bitnami/kafka/bin/kafka-topics.sh -create --topic $topic --bootstrap-server localhost:9092
done

kubectl apply -f SystemCharts/TestingPod.yaml
kubectl wait --for=condition=Ready --timeout=6000s pod tests 

for dir in ./Source/*/Services/*/
do
    dir_name=$(basename $dir)
    service_name=$(echo $dir_name | tr '[:upper:]' '[:lower:]')
    #Deploy service
    helm install $service_name ./SystemCharts/ServiceChart \
        --namespace development \
        -f ./SystemCharts/ServiceChart/ServiceValues/$dir_name.values.yaml  \
        --wait
done

sh Scripts/ForwardPorts.sh