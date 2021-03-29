i=9000
for dir in ./Source/NodeJS/Services/*/
do
    dir_name=$(basename $dir)
    service_name=$(echo $dir_name | tr '[:upper:]' '[:lower:]')
    kubectl port-forward service/$service_name $i:9229 & 
    i=$((i+1))
done