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
 -e "EggPlantApiDB=EggPlant" -e "ElasticSearchUrl=http://localhost:9200" 
 eggplant-api:latest
```

## 2. Addresses
* Elasticsearch: http://localhost:9200/
* Kibana: http://localhost:5601/
* RabbitMQ: http://localhost:9080
* RavebDB: http://localhost:8000
