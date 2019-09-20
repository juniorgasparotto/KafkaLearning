using Microsoft.Extensions.DependencyInjection;

namespace KafkaLearning.ServiceBus
{
    public class KafkaBuilder
    {
        public IServiceCollection Services { get; }

        public KafkaBuilder(IServiceCollection serviceCollection)
        {
            this.Services = serviceCollection;
        }
    }
}
