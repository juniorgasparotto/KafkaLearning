using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using KafkaLearning.ServiceBus.Extensions;
using KafkaLearning.ServiceBus.Kafka.Producer;
using KafkaLearning.ServiceBus.Logs;
using KafkaLearning.ServiceBus.Helpers;

namespace KafkaLearning.ServiceBus.Kafka.Consumer
{
    public class RedirectConsumerClient<TKey, TValue> : IConsumerClient<TKey, TValue>
    {
        private CancellationToken cancellationToken;
        private readonly string redirectTopic;
        private readonly IProducerSender<TKey, TValue> retrySender;
        private readonly IServiceBusLogger logger;
        private readonly int delay;
        private readonly IConsumerClient<TKey, TValue> consumerClient;

        public RedirectConsumerClient(
            string redirectTopic,
            int delay,
            IConsumerClient<TKey, TValue> consumerClient,
            IProducerSender<TKey, TValue> retrySender,
            IServiceBusLogger logger
        )
        {
            this.redirectTopic = redirectTopic;
            this.delay = delay;
            this.consumerClient = consumerClient;
            this.retrySender = retrySender;
            this.logger = logger;
        }

        public void Subscribed(IConsumer<TKey, TValue> consumer, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.consumerClient?.Subscribed(consumer, cancellationToken);
        }

        public void MessageReceived(ConsumeResult<TKey, TValue> result)
        {
            if (delay > 0)
            {
                // this.consumer.Pause(this.consumer.Assignment);
                logger.Log($"Start delay: {delay}ms ({DateTime.Now.ToString("HH:mm:ss")})");
                DelayHelper.DelayMessage(result.Timestamp.UnixTimestampMs, delay, cancellationToken);
                logger.Log($"Stop delay: {delay}ms ({DateTime.Now.ToString("HH:mm:ss")})");
                // this.consumer.Resume(this.consumer.Assignment);
            }

            this.Resend(result, this.redirectTopic);
            this.consumerClient?.MessageReceived(result);
        }

        public void MessageReceivedExceptionOcurred(ConsumeResult<TKey, TValue> result, Exception e)
        {
            this.consumerClient?.MessageReceivedExceptionOcurred(result, e);
        }

        // DUVIDA: Em que cenário seria usado?
        public void AutoCommited(CommittedOffsets offsets)
        {
            this.consumerClient?.AutoCommited(offsets);
        }

        public void Commited(IEnumerable<TopicPartitionOffset> offsets)
        {
            this.consumerClient?.Commited(offsets);
        }

        public void ConsumeExceptionOcurred(ConsumeException e)
        {
            this.consumerClient?.ConsumeExceptionOcurred(e);
        }

        private void Resend(ConsumeResult<TKey, TValue> cr, string topic)
        {
            retrySender.SendAsync(cr, topic).Wait();
        }
    }
}
