using KafkaLearning.ServiceBus.Configurations;
using KafkaLearning.Web.Infrastructure.Configurations;

namespace KafkaLearning.Web.Infrastructure.Configuration
{
    public class KafkaOptions : KafkaConfig
    {
        public ProducersOptions Producers { get; set; }
        public ConsumersOptions Consumers { get; set; }

        public class ProducersOptions
        {
            public ProducerOptions TopicSample { get; set; }
        }

        public class ConsumersOptions
        {
            public ConsumerOptions TopicSample { get; set; }
        }
    }

}
