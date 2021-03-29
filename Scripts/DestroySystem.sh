helm uninstall cluster-resources --namespace development

#Remove all services
for dir in ./Source/*/Services/*/
do
    dir_name=$(basename $dir)
    service_name=$(echo $dir_name | tr '[:upper:]' '[:lower:]')
    helm uninstall $service_name --namespace development
done

kubectl delete namespace development

sh Scripts/StopForwardingPorts.sh