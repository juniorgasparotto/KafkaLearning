using KafkaLearning.ServiceBus.Configurations;
using KafkaLearning.Web.Core.Entities;

namespace KafkaLearning.Web.Infrastructure.ViewModel
{
    public class PublisherRequest
    {
        public ProducerOptions Settings { get; set; }
        public ChatMessage Message { get; set; }
    }
}