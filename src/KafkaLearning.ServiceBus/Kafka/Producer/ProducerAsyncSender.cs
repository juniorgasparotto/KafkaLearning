using Confluent.Kafka;
using KafkaLearning.ServiceBus.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaLearning.ServiceBus.Kafka.Producer
{
    public class ProducerAsyncSender<TKey, TValue> : IProducerSender<TKey, TValue>
    {
        private readonly IProducer<TKey, TValue> producer;
        private readonly IProducerClient<TKey, TValue> producerClient;
        private readonly IServiceBusLogger logger;

        public ProducerAsyncSender(
            IProducer<TKey, TValue> producer,
            IProducerClient<TKey, TValue> producerClient,
            IServiceBusLogger logger)
        {
            this.producer = producer;
            this.producerClient = producerClient;
            this.logger = logger;
        }

        public Task SendAsync(ConsumeResult<TKey, TValue> cr, string topicName, CancellationToken cancellationToken = default)
        {
            return SendAsync(cr.Key, cr.Message.Value, topicName, cr.Headers, cancellationToken);
        }

        public Task SendAsync(TKey key, TValue value, string topicName, Headers headers = null, CancellationToken cancellationToken = default)
        {
            var message = new Message<TKey, TValue>
            {
                Key = key,
                Value = value,
                Headers = headers
            };

            // DUVIDA: poderia usar um task tipado com async no método para simplificar a leitura do código?
            var task = producer.ProduceAsync(topicName, message)
                .ContinueWith(async m => producerClient?.MessageDelivered(await m), cancellationToken);

            // DUVIDA: Na prática, não fica redundante com o MessageDelivered?
            producerClient?.MessageSent(message);

            this.logger.Log($"[To queue]: Message delivered: {topicName}, {message.Key}");
            return task;
        }
    }
}