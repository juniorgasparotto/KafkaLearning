using System.Threading;
using KafkaLearning.ServiceBus.Configurations;

namespace KafkaLearning.Web.Infrastructure.ViewModel
{
    public class AppInfo
    {
        public string AppName { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
        public int TaskId { get; set; }
        public bool SimulateError { get; set; }
        public ConsumerOptions Settings { get; set; }
    }
}