using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaLearning.ServiceBus.Kafka.Producer
{
    public interface IProducerSender<TKey, TValue>
    {
        Task SendAsync(TKey key, TValue value, string topicName, Headers headers = null, CancellationToken cancellationToken = default(CancellationToken));
        Task SendAsync(ConsumeResult<TKey, TValue> cr, string topicName, CancellationToken cancellationToken = default(CancellationToken));
    }
}