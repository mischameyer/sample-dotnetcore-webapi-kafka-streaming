# sample-dotnetcore-webapi-kafka-streaming
A simple .net core webapi using Kafka as a streaming API.

## StreamingApi (dotnet core webapi)
The webapi is the entry point to generate an order (i.e. to order a book). The Api sends the request (after serializing the object to a json string) to a topic in Kafka called 'orderBookRequests'.

- Endpoint: http://localhost:5000/api/order

- Action: POST

- Body: ``` {"Id":1234, "Book":{"Title":"Origin", "Isbn": "9123456123456"}, "Quantity":3} ```

## StreamingDistributor (dotnet core console app)
The console app is used as a running service for mapping (from OrderRequest to DeliverRequest) and distributing. As soon as the app is started it acts as a Kafka consumer with a subscription to the topic 'orderBookRequests'. When receiving the messages, they are desirialized from a json string and mapped into a new object called 'DeliveryOrder'. This object will be sent to a producer which writes messages to the Kafka topic 'deliverBookRequests'.

## Producer (dotnet core class library)
This class library has two producers. One producer writes to the Kafka topic 'orderBookRequests', the other producer writes to the Kafka topic 'deliverBookRequests'.

## Option 1 (manually): Start Zookeeper and Kafka-Server manually

### Precon:
Install Kafka (latest version) on your local machine: [https://kafka.apache.org/downloads]

### Step 1.1: Start the Zookeeper Server (in separate Terminal):
```bin\windows\zookeeper-server-start.bat config/zookeeper.properties```

### Step 1.2: Start the Kafka Server (in separate Terminal):
```bin\windows\kafka-server-start.bat config/server.properties```

## Option 2 (with Docker): Start Zookeeper, Kafka-Server, Broker and Confluent with Docker

### Step 2.1: Clone Confluent-Examples from Github
```git clone https://github.com/confluentinc/examples/tree/5.3.1-post/cp-all-in-one```

### Step 2.2: Docker-Compose
Make sure you have at least 8192MB Memory allocated in docker (Settings, Advanced).

```cd cp-all-in-one```

```docker-compose up -d --build```

### Step 2.3: Check State of Docker-Containers
```docker-compose ps```

All states should have the value 'Up'

### Step 2.4: Use Confluent Control Center
Go to a webbrowser and start Confluent Control Center: ```localhost:9021```

Under 'topics' you can create two new topics called 'orderBookRequests' and 'deliverBookRequests' with 1 partitions each.


### Step 3: Start the .NET Console StreamingDistributor (in separate Terminal):

```cd StreamingDistributor```

dotnet run args[-topic]  (i.e. ```dotnet run -topic orderBookRequests```)

The result should be like that:

- Press Ctrl+C to exit
- Topic: orderBookRequests
- Group: test-group

### Step 4: Start the .NET WebApi StreamingApi (in separate Terminal):

```cd StreamingApi```

```dotnet run```

The result should be like that:

- info: Microsoft.Hosting.Lifetime[0]
-      Now listening on: https://localhost:5001
- info: Microsoft.Hosting.Lifetime[0]
-      Now listening on: http://localhost:5000
- info: Microsoft.Hosting.Lifetime[0]
-      Application started. Press Ctrl+C to shut down.
- info: Microsoft.Hosting.Lifetime[0]
-      Hosting environment: Development
- info: Microsoft.Hosting.Lifetime[0]

### Step 5: Generate a new order with a common REST-Api Tool (i.e. Insomnia)

https: ```https://localhost:5001```

http: ```http://localhost:5000```

Action: POST

Header: application/json

Body: ``` {"Id":1234, "Book":{"Title":"Origin", "Isbn": "9123456123456"}, "Quantity":3} ```

The result in the console (where the WebApi is running) should be like that:

Delivered '{"Id":1234,"Book":{"Isbn":"9123456123456","Title":"Origin"},"Quantity":3,"status":0}' to: orderBookRequests [[0]] @47
