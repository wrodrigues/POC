using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using POC.Web.Kafka.Consumer.Configs.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace POC.Web.Kafka.Consumer.Services.Background
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ServiceConfig ServiceConfig;
        private List<IObserver> Observers;

        public KafkaConsumerService(ServiceConfig serviceConfig, List<IObserver> observers = null)
        {
            ServiceConfig = serviceConfig;
            Observers = observers ?? new List<IObserver>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConsumerConfig kafkaConsumerConfig = await GetKafkaConsumerConfig(ServiceConfig);

            await ConsumeKafkaMessagesAsync(stoppingToken, kafkaConsumerConfig, ServiceConfig);
        }

        private Task<ConsumerConfig> GetKafkaConsumerConfig(ServiceConfig serviceConfig)
        {
            return Task.FromResult(new ConsumerConfig
            {
                BootstrapServers = serviceConfig.Server,
                GroupId = serviceConfig.Group,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            });
        }

        private Task ConsumeKafkaMessagesAsync(CancellationToken stoppingToken, ConsumerConfig consumerConfig, ServiceConfig serviceConfig)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
                {
                    consumer.Subscribe(serviceConfig.Topic);

                    ConsumeResult<Null, string> consumerResult = null;

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        do
                        {
                            consumerResult = consumer.Consume(TimeSpan.FromSeconds(serviceConfig.Timeout));

                        } while (consumerResult == null || consumerResult.Message == null || string.IsNullOrEmpty(consumerResult.Message.Value));

                        NotifyObservers(consumerResult);
                    }

                    consumer.Unsubscribe();

                    consumer.Close();
                }
            });
        }

        private void NotifyObservers(ConsumeResult<Null, string> consumerResult)
        {
            Observers.ForEach((observer) =>
            {
                observer.Notify(consumerResult.Message.Value);
            });
        }
    }
}
