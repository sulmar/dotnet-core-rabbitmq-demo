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

### Zarządzanie

Wpisz w przeglądarce

~~~
http://localhost:15672
~~~


domyślnie hasło i login to „guest”

###


| atrybut      | opis         |
| -------------|--------------|
| content type | typ danych   |
| content encoding | kodowanie danych |
| routing key | klucz routing-u |
| delivery mode | sposób dostarczenia wiadomości określający, czy wiadomość ma być utrwalona (ang. persistent) |
| message priority | priorytet wiadomości |
| message publishing timestamp | czas publikacji wiadomości |
| expiration period | inaczej TTL czyli opóźnienie w dostarczeniu wiadomości podawane w milisekundach |
| publisher application id | identyfikator producenta |
