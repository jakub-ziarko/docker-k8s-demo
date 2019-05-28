# docker-k8s-demo

## 1. Start whole stack
`docker-compose -f "docker-compose.yml" -f "docker-compose.dev.yml" up`

### Build only one container
`docker-compose build --no-cache eggplant-api`

### Run build image
```
 docker run -it -p "5000:80" 
 -e "ASPNETCORE_ENVIRONMENT=Development" 
 -e "ASPNETCORE_URLS=http://+:80" -e "EggPlantApiDBUrl=http://localhost:8000" 
 -e "EggPlantApiDB=EggPlant" -e "ElasticSearchUri=http://localhost:9200" 
 eggplant-api:latest
```

### Azure and Kubernetes

Log to azure:
`az login`

Browse azure Kubernetes cluster:
`az aks browse --resource-group docker-k8s-demo --name ClusterName`

Create kune.yml based on docker-compose:
https://kubernetes.io/docs/tasks/configure-pod-container/translate-compose-kubernetes/

`kompose convert -f docker-compose.yml -f docker-compose.dev.yml`

```kubectl apply -f eggplant-api-deployment.yaml -f eggplant-api-service.yaml -f elasticsearch-deployment.yaml -f elasticsearch-service.yaml -f kibana-deployment.yaml -f kibana-service.yaml -f orders-api-deployment.yaml -f orders-api-service.yaml -f rabbit-mq-mgmt-deployment.yaml -f rabbit-mq-mgmt-service.yaml -f ravendb-deployment.yaml -f ravendb-service.yaml```

### Minikube

`minikube start --vm-driver=host`

`minikube dashboard`

`minikube stop`

## 2. Addresses
* Elasticsearch: http://localhost:9200/
* Kibana: http://localhost:5601/
* RabbitMQ: http://localhost:9080
* RavebDB: http://localhost:8000
