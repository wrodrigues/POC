using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POC.Web.Kafka.Consumer.Configs.Kafka;
using POC.Web.Kafka.Consumer.Services.Background;

namespace POC.Web.Kafka.Consumer
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddConsumerBackgroundService(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddHostedService((serviceProvider) =>
            {
                var serviceConfig = configuration.GetSection("POC:Kafka:Consumer").Get<ServiceConfig>();

                var observers = new List<IObserver>();

                observers.Add(new Observer());

                return new KafkaConsumerService(serviceConfig, observers);
            });
        }
    }
}
