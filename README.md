# dotnet-core-rabbitmq-demo

## Instalacja RabbitMQ 

### Docker
~~~ bash
docker run -d --hostname my-rabbit --name some-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
~~~


docker-compose.yml
~~~
version: '2'
 
services:
  rabbit:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
~~~

~~~
docker-compose up
~~~
