using Confluent.Kafka;
using KafkaLearning.ServiceBus.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaLearning.ServiceBus.Kafka.Producer
{
    public class ProducerSyncSender<TKey, TValue> : IProducerSender<TKey, TValue>
    {
        private readonly IProducer<TKey, TValue> producer;
        private readonly IProducerClient<TKey, TValue> producerClient;
        private readonly IServiceBusLogger logger;

        public ProducerSyncSender(
            IProducer<TKey, TValue> producer,
            IProducerClient<TKey, TValue> producerClient,
            IServiceBusLogger logger
        )
        {
            this.producer = producer;
            this.producerClient = producerClient;
            this.logger = logger;
        }

        public Task SendAsync(ConsumeResult<TKey, TValue> cr, string topicName, CancellationToken cancellationToken = default)
        {
            return SendAsync(cr.Key, cr.Message.Value, topicName, cr.Headers, cancellationToken);
        }

        // DUVIDA: O nome do método é Async dentro de uma classe Sync, talvez juntar esse método no ProducerAsyncSender e renome-lo para 
        // ProducerSender com os dois métodos?
        public async Task SendAsync(TKey key, TValue value, string topicName, Headers headers = null, CancellationToken cancellationToken = default)
        {
            var message = new Message<TKey, TValue>
            {
                Key = key,
                Value = value,
                Headers = headers
            };

            var produceTask = producer.ProduceAsync(topicName, message);
            producerClient.MessageSent(message);
            producerClient.MessageDelivered(await produceTask);

            // DUVIDA: Faz sentido ter o flush no método asyncrono tb? talvez deixar isso no ProducerSettings.
            producer.Flush(cancellationToken);
            this.logger.Log($"[To queue]: Message delivered: {topicName}, {message.Key}");
        }

    }
}
