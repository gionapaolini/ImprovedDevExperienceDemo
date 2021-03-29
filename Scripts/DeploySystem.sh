cd $(dirname "$0")
cd .. 

kubectl create namespace development
kubectl config set-context --current --namespace=development

helm install cluster-resources ./SystemCharts/ClusterResources --namespace development --wait

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



i=9000
for dir in ./Source/NodeJS/Services/*/
do
    dir_name=$(basename $dir)
    service_name=$(echo $dir_name | tr '[:upper:]' '[:lower:]')
    kubectl port-forward service/$service_name $i:9229 & 
    i=$((i+1))
done