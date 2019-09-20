using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;
using KafkaLearning.ServiceBus.Extensions;
using KafkaLearning.ServiceBus.Helpers;
using KafkaLearning.ServiceBus.Kafka.Producer;
using KafkaLearning.ServiceBus.Logs;

namespace KafkaLearning.ServiceBus.Kafka.Consumer
{
    public class RetryConsumerClient<TKey, TValue> : IConsumerClient<TKey, TValue>
    {
        private int consumed;
        private CancellationToken cancellationToken;
        private IConsumer<TKey, TValue> consumer;
        private readonly string retryTopic;
        private readonly IConsumerClient<TKey, TValue> consumerClient;
        private readonly IProducerSender<TKey, TValue> retrySender;
        private readonly IServiceBusLogger logger;
        private readonly string groupId;
        private readonly int delay;

        public RetryConsumerClient(
            string retryTopic,
            string groupId,
            int delay,
            IConsumerClient<TKey, TValue> consumerClient,
            IProducerSender<TKey, TValue> retrySender,
            IServiceBusLogger logger
        )
        {
            // DUVIDA: não entendi o uso dessa variavel
            this.consumed = 0;

            this.retryTopic = retryTopic;
            this.groupId = groupId;
            this.delay = delay;
            this.consumerClient = consumerClient;
            this.retrySender = retrySender;
            this.logger = logger;
        }

        public void Subscribed(IConsumer<TKey, TValue> consumer, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.consumer = consumer;
            this.consumerClient.Subscribed(consumer, cancellationToken);
        }

        // DUVIDA: Em que cenário seria usado?
        public void AutoCommited(CommittedOffsets offsets)
        {
            this.consumerClient.AutoCommited(offsets);
        }

        public void Commited(IEnumerable<TopicPartitionOffset> offsets)
        {
            this.consumerClient.Commited(offsets);
        }

        public void ConsumeExceptionOcurred(ConsumeException e)
        {
            this.consumerClient.ConsumeExceptionOcurred(e);
        }

        public void MessageReceived(ConsumeResult<TKey, TValue> result)
        {
            if (delay > 0)
            {
                // this.consumer.Pause(this.consumer.Assignment); // tentativa mal sucessidada de evitar o problema de max.pull.ms que é menor que o tempo da task
                logger.Log($"Start delay: {delay}ms ({DateTime.Now.ToString("HH:mm:ss")})");
                DelayHelper.DelayMessage(result.Timestamp.UnixTimestampMs, delay, cancellationToken);
                logger.Log($"Stop delay: {delay}ms ({DateTime.Now.ToString("HH:mm:ss")})");
                // this.consumer.Resume(this.consumer.Assignment); // tentativa mal sucessidada de evitar o problema de max.pull.ms que é menor que o tempo da task
            }

            this.consumerClient.MessageReceived(result);
        }

        public void MessageReceivedExceptionOcurred(ConsumeResult<TKey, TValue> result, Exception e)
        {
            Retry(result);
            this.consumerClient.MessageReceivedExceptionOcurred(result, e);
        }

        private void Retry(ConsumeResult<TKey, TValue> cr)
        {
            // caso seja a primeira leitura com erro dessa mensagem, então
            // adiciona o nome original do grupo de consumidor para que somente esse grupo
            // de consumidor possa ler essa mensagem futuramente
            cr.Headers.SetOriginalGroupIdIfNotExists(groupId);

            // Esse bloco de código é usado quando o modelo de retry é o que sempre volta
            // para o consumidor principal (tópico original).
            // Caso exista mais de um tópico de retry, verificar para qual enviar com base
            // no header RETRY_COUNT da mensagem
            var retryTopics = retryTopic.Split(new char[] { ';', ',' });
            if (retryTopics.Length > 1)
            {
                // Obtem o número atual de tentativas
                var retryCount = cr.Headers.GetRetryCount();

                // var maxRetry = retryTopics.Length;
                // if (maxRetry > 0 && retryCount >= maxRetry)
                //     return;

                cr.Headers.IncrementRetryCount();
                Resend(cr, retryTopics[retryCount]);
            }
            else
            {
                Resend(cr, retryTopic);
            }
        }

        private void Resend(ConsumeResult<TKey, TValue> cr, string topic)
        {
            retrySender.SendAsync(cr, topic).Wait();
        }
    }
}
