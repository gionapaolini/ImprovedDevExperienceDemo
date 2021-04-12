using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace QAEditor.Helpers
{
    public interface IKafkaService
    {
        Task ProduceMessage<T>(string topic, T value);
        void StartConsuming(string groupId, Dictionary<string, Func<string, Task>> topicHandlers);
    }

    public class KafkaService : IKafkaService
    {
        readonly string ServerHosts;
        IProducer<Null, string> _producer;

        public KafkaService(string serverHosts)
        {
            ServerHosts = serverHosts;
        }

        public async Task ProduceMessage<T>(string topic, T value)
        {
            if (_producer == null)
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = ServerHosts
                };
                _producer = new ProducerBuilder<Null, string>(config).Build();
            }
            var jsonValue = JsonSerializer.Serialize(value);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonValue });
        }

        public void StartConsuming(string groupId, Dictionary<string, Func<string, Task>> topicHandlers)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = ServerHosts,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            Task.Run(() =>
            {
                using var consumer = new ConsumerBuilder<string, string>(config).Build();
                foreach (var topic in topicHandlers)
                {
                    consumer.Subscribe(topic.Key);
                }

                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume();
                        if (topicHandlers.TryGetValue(consumeResult.Topic, out Func<string, Task> handler))
                        {
                            handler(consumeResult.Message.Value);
                        }
                        else
                        {
                            throw new Exception($"There is no handler for topic {consumeResult.Message.Key}");
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error occured: {e.Message}");
                    }
                }
            });
        }
    }


}