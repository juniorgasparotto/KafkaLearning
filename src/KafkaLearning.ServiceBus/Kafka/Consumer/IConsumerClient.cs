using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;

namespace KafkaLearning.ServiceBus.Kafka.Consumer
{
    public interface IConsumerClient<TKey, TValue> 
    {
        void Subscribed(IConsumer<TKey, TValue> consumer, CancellationToken cancellationToken);
        void AutoCommited(CommittedOffsets offsets);
        void Commited(IEnumerable<TopicPartitionOffset> offsets);
        void MessageReceived(ConsumeResult<TKey, TValue> result);
        void ConsumeExceptionOcurred(ConsumeException e);
        void MessageReceivedExceptionOcurred(ConsumeResult<TKey, TValue> result, Exception e);
    }
}