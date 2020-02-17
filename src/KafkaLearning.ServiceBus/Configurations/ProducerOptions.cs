namespace KafkaLearning.ServiceBus.Configurations
{
    public class ProducerOptions
    {
        public string Topic { get; set; }
        public string BootstrapServers { get; set; }
        public bool IgnoreSsl { get; set; }

    }
}