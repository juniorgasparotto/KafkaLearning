using KafkaLearning.ServiceBus.Configurations;

namespace KafkaLearning.Web.Infrastructure.Configuration
{
    public class KafkaOptions
    {
        public ProducersOptions Producers { get; set; }
        public ConsumersOptions Consumers { get; set; }

        public class ProducersOptions
        {
            public ProducerOptions Chat { get; set; }
        }

        public class ConsumersOptions
        {
            public ConsumerOptions Chat { get; set; }
        }
    }

}
