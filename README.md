# sample-dotnetcore-webapi-kafka-streaming
A simple .net core webapi using Kafka as a streaming API.

## Streaming Api (dotnet core webapi)
The webapi is the entry point to generate an order (i.e. to order a book). The Api sends the request (after serializing the object to a json string) to a topic in Kafka called 'orderBookRequests'.

- Endpoint: http://localhost:3480/api/order

- Action: POST

- JSON Body: ``` {"Id":1234, "Book":{"Title":"Origin", "Isbn": "9123456123456"}, "Quantity":3} ```

## Streaming Distributor (dotnet core console app)
The console app is used as a running service for mapping (from OrderRequest to DeliverRequest) and distributing. As soon as the app is started it acts as a Kafka consumer with a subscription to the topic 'orderBookRequests'. When receiving the messages, they are desirialized from a json string and mapped into a new object called 'DeliveryOrder'. This object will be sent to a producer which writes messages to the Kafka topic 'deliverBookRequests'.

## Producer (dotnet core class library)
This class library hat two producers. One producer writes to the Kafka topic 'orderBookRequests', the other producer writes to the Kafka topic 'deliverBookRequests'.



