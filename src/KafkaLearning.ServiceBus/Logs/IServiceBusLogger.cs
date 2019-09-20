namespace KafkaLearning.ServiceBus.Logs
{
    public interface IServiceBusLogger
    {
        void Log(string message);
    }
}