using System.Threading;

namespace KafkaLearning.Web.Infrastructure.ViewModel
{
    public class AppInfo
    {
        public string AppName { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
        public int TaskId { get; set; }
    }
}