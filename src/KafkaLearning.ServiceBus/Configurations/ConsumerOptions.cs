namespace KafkaLearning.ServiceBus.Configurations
{
    public class ConsumerOptions
    {
        public string Topic { get; set; }
        public string RetryTopic { get; set; }
        public string RetryStrategy { get; set; }


        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public bool EnableAutoCommit { get; set; }
        public int AutoOffSetReset { get; set; }
        public bool EnablePartitionEof { get; set; }
        public int MaxPollIntervalMs { get; set; }
        public int Delay { get; set; }
        public bool IgnoreSsl { get; set; }
    }
}