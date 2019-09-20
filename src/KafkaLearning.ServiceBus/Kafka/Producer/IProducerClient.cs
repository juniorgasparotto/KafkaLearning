using Confluent.Kafka;

namespace KafkaLearning.ServiceBus.Kafka.Producer
{
    public interface IProducerClient<TKey, TValue> 
    {
        void MessageSent(Message<TKey, TValue> message);
        void MessageDelivered(DeliveryResult<TKey, TValue> deliveryResult);
    }
}