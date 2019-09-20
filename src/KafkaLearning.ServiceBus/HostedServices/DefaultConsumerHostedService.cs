using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KafkaLearning.ServiceBus.Kafka.Consumer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaLearning.ServiceBus.HostedServices
{
    public class DefaultConsumerHostedService<TKey, TValue> : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ITopicConsumer<TKey, TValue> topicConsumer;
        //private readonly CancellationTokenSource cts;

        public DefaultConsumerHostedService(
            ITopicConsumer<TKey, TValue> topicConsumer,
            ILogger<DefaultConsumerHostedService<TKey, TValue>> logger
        )
        {
            this._logger = logger;
            this.topicConsumer = topicConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start consume: {topicConsumer.ToString()}");
            //topicConsumer.Run(this.cts.Token);
            topicConsumer.Run(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //this.cts.Cancel();
            _logger.LogInformation($"Stop consume: {topicConsumer.ToString()}");
            return Task.CompletedTask;
        }
    }
}
