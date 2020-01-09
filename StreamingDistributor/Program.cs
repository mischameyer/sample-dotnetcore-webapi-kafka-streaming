using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StreamingService.Service;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StreamingDistributor
{
    class Program
    {
        static void Main(string[] args)
        {

            var switchMappings = new Dictionary<string, string>()
             {
                 { "-topic", "key1" },
                 { "-group", "key2" },
             };
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args, switchMappings);

            var config = builder.Build();

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IProducerService2, ProducerService2>()                
                .BuildServiceProvider();

            CancellationTokenSource cts = new CancellationTokenSource();
            ConsumerConfig consumerConfig = null;

            CreateConfig();
            CreateConsumerAndConsume();


            void CreateConfig()
            {
                consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = string.IsNullOrWhiteSpace(config["key2"]) ? "test-group" : config["key2"],
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };
            }

            void CreateConsumerAndConsume()
            {

                var cb = new ConsumerBuilder<string, string>(consumerConfig);

                Console.WriteLine("Press Ctrl+C to exit");

                Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);

                using (var consumer = cb.Build())
                {
                    var _topic = !string.IsNullOrWhiteSpace(config["key1"]) ? config["key1"] : "orderBookRequests";
                    consumer.Subscribe(_topic);
                    Console.WriteLine(string.Format("Topic: {0}", _topic));
                    Console.WriteLine(string.Format("Group: {0}", consumerConfig.GroupId));

                    try
                    {
                        while (!cts.IsCancellationRequested)
                        {
                            var cr = consumer.Consume(cts.Token);
                            var offset = cr.TopicPartitionOffset;
                            Console.WriteLine($"Message '{cr.Value}' at: '{offset}'.");
                            //do the actual work here
                            var ps2 = serviceProvider.GetService<IProducerService2>();
                            ps2.SendMessage("deliverBookRequests", cr.Value, true, 1);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        consumer.Close();
                    }
                }
            }

            void OnExit(object sender, ConsoleCancelEventArgs args)
            {
                args.Cancel = true;
                Console.WriteLine("In OnExit");
                cts.Cancel();

            }

        }
    }
}
