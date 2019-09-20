using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;

namespace KafkaLearning.Web.Infrastructure.Logs
{
    public class TaskIdEnricher : ILogEventEnricher
        {
            /// <summary>
            /// The property name added to enriched log events.
            /// </summary>
            public const string TaskIdPropertyName = "TaskId";

            /// <summary>
            /// The cached last created "ThreadId" property with some thread id. It is likely to be reused frequently so avoiding heap allocations.
            /// </summary>
            private LogEventProperty _lastValue;

            /// <summary>
            /// Enrich the log event.
            /// </summary>
            /// <param name="logEvent">The log event to enrich.</param>
            /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var taskId = Task.CurrentId;
                if (taskId == null) return;
                
                var last = _lastValue;
                if (last == null || (int)((ScalarValue)last.Value).Value != taskId)
                    // no need to synchronize threads on write - just some of them will win
                    _lastValue = last = new LogEventProperty(TaskIdPropertyName, new ScalarValue(taskId));

                logEvent.AddPropertyIfAbsent(last);
            }
        }
}