using System.Threading;
using System.Threading.Tasks;

namespace KafkaLearning.ServiceBus.Kafka.Consumer
{
    public interface ITopicConsumer<TKey, TValue>
    {
        Task Run(CancellationToken cancellationToken = default);
    }
}