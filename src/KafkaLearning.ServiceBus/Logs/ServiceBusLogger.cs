using Microsoft.Extensions.Logging;

namespace KafkaLearning.ServiceBus.Logs
{
    public class ServiceBusLogger : IServiceBusLogger
    {
        private readonly ILogger<object> logger;

        public ServiceBusLogger(ILogger<object> logger)
        {
            this.logger = logger;
        }

        public void Log(string message)
        {
            this.logger.LogInformation(message);
        }
    }
}