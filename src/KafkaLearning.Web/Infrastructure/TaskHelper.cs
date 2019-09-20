using System;
using System.Threading.Tasks;

namespace KafkaLearning.Web.Infrastructure
{
    public static class TaskHelper
    {
        public static async Task SetInterval(Action action, int timeout)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(timeout)).ConfigureAwait(false);
            action();
            SetInterval(action, timeout);
        }
    }
}