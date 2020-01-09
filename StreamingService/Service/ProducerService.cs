using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingService.Service
{
    public class ProducerService : IProducerService
    {        
        private readonly ProducerConfig _producerConfig;
        private IProducer<string, string> producer = null;

        public ProducerService(ProducerConfig producerConfig)
        {
            _producerConfig = producerConfig;

            CreateProducer();
        }
       
        public void CreateProducer()
        {
            var pb = new ProducerBuilder<string, string>(_producerConfig);
            producer = pb.Build();
        }

        public async Task<string> SendMessage(string topic, string message, bool display, int key)
        {
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
