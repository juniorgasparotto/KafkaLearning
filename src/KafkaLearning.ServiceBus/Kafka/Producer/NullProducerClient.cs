using Confluent.Kafka;
using KafkaLearning.ServiceBus.Kafka.Producer;
using KafkaLearning.ServiceBus.Logs;

namespace KafkaLearning.ServiceBus.Kafka.Producer
{
    internal class NullProducerClient<TKey, TValue> : IProducerClient<TKey, TValue>
    {
        private readonly IServiceBusLogger logger;

        public NullProducerClient(IServiceBusLogger logger)
        {
            this.logger = logger;
        }

        public void MessageDelivered(DeliveryResult<TKey, TValue> deliveryResult)
        {
            
        }

        public void MessageSent(Message<TKey, TValue> message)
        {
            
        }
    }
}