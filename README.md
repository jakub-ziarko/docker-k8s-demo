# docker-k8s-demo

## 1. Start whole stack
`docker-compose -f "docker-compose.yml" -f "docker-compose.dev.yml" up`

### Build only one container
`docker-compose build eggplant-api`

## 2. Addresses
* Elasticsearch: http://localhost:9200/
* Kibana: http://localhost:5601/
* RabbitMQ: http://localhost:9080
* RavebDB: http://localhost:8000