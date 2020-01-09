using Confluent.Kafka;
using Newtonsoft.Json;
using StreamingService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StreamingService.Service
{
    public class ProducerService2 : IProducerService2
    {        
        private IProducer<string, string> producer = null;
        private ProducerConfig producerConfig = null;

        public ProducerService2()
        {            
            CreateConfig();
            CreateProducer();
        }

        public void CreateProducer()
        {
            var pb = new ProducerBuilder<string, string>(producerConfig);
            producer = pb.Build();
        }
        void CreateConfig()
        {
            producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };
        }


        public async Task<string> SendMessage(string topic, string order, bool display, int key)
        {
            var orderObject = JsonConvert.DeserializeObject<OrderRequest>(order);

            var deliveryObject = new OrderDelivery { OrderRequestId = orderObject.Id, CurrentLocation = "in store", DeliveredBy = "Donald Duck" };

            var message = JsonConvert.SerializeObject(deliveryObject);

            var msg = new Message<string, string>
            {
                Key = key.ToString(),
                Value = message
            };

            DeliveryResult<string, string> delRep;

            if (key > 1)
            {
                var p = new Partition(key);
                var tp = new TopicPartition(topic, p);
                delRep = await producer.ProduceAsync(tp, msg);
            }
            else
            {
                delRep = await producer.ProduceAsync(topic, msg);
            }

            var topicOffset = delRep.TopicPartitionOffset;

            if (display) { Console.WriteLine($"Delivered '{delRep.Value}' to: {topicOffset}"); }

            return message;
        }


    }
}
