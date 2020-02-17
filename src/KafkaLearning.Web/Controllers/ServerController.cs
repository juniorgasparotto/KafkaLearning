using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Events;
using Serilog.Formatting.Compact.Reader;
using KafkaLearning.ServiceBus;
using KafkaLearning.ServiceBus.Configurations;
using KafkaLearning.ServiceBus.Kafka.Consumer;
using KafkaLearning.ServiceBus.Kafka.Producer;
using KafkaLearning.ServiceBus.Logs;
using KafkaLearning.ServiceBus.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using KafkaLearning.Web.Core.Entities;
using KafkaLearning.Web.Hubs;
using KafkaLearning.Web.Infrastructure;
using KafkaLearning.Web.Infrastructure.Configurations;
using KafkaLearning.Web.Infrastructure.ConsumersClients;
using KafkaLearning.Web.Infrastructure.ViewModel;
using System.Linq;

namespace KafkaLearning.Web.Controllers
{
    [Route("api/[controller]")]
    public partial class ServerController : Controller
    {
        private static ConcurrentDictionary<string, AppInfo> subscribersCancels = new ConcurrentDictionary<string, AppInfo>();

        private readonly IHubContext<EventMessageHub> _kafkaHubContext;
        private readonly IHubContext<LogHub> _logHubContext;
        private readonly AppConfigurationOptions _appConfiguration;
        private readonly ILogger<ServerController> _logger;

        public ServerController(
            ILogger<ServerController> logger,
            IOptions<AppConfigurationOptions> appConfiguration,
            IHubContext<EventMessageHub> kafkaHubContext,
            IHubContext<LogHub> logHubContext,
            IOptions<KafkaConfig> op
        )
        {
            this._kafkaHubContext = kafkaHubContext;
            this._logHubContext = logHubContext;
            this._appConfiguration = appConfiguration.Value;
            this._logger = logger;
        }


        [HttpGet("[action]")]
        public void EnableError(string appName, bool value)
        {
            if (subscribersCancels.ContainsKey(appName))
                subscribersCancels[appName].SimulateError = value;
        }

        [HttpGet("[action]")]
        public void UnSubscribeAll()
        {
            foreach (var c in subscribersCancels)
                c.Value.CancellationToken.Cancel();
            subscribersCancels.Clear();
        }

        [HttpGet("[action]")]
        public IEnumerable<AppInfo> GetSubscribers(string appName = null)
        {
            return subscribersCancels.Values.Where(f=> appName == null || f.AppName == appName);
        }

        [HttpPost("[action]")]
        public async void Send(
            [FromBody]PublisherRequest request,
            [FromServices] IServiceBusLogger logger
        )
        {
            // Set message properties
            request.Message.Id = Guid.NewGuid();
            request.Message.SendDate = DateTime.Now;

            // Create producer
            var builder = new ProducerConnectionBuilder<Guid, EventMessage>(this._appConfiguration.Kafka.CertificatePath);
            var producer = builder
                            .WithBootstrapServers(request.Settings.BootstrapServers)
                            .WithAsyncProducer()
                            .WithJsonSerializer()
                            .Build();
            var producerSender = new ProducerAsyncSender<Guid, EventMessage>(
                producer,
                null,
                logger
            );

            await producerSender.SendAsync(request.Message.Id, request.Message, request.Settings.Topic);
        }

        [HttpPost("[action]")]
        public IActionResult Subscribe(
            string appName,
            bool simulateError,
            [FromBody]ConsumerOptions settings,
            [FromServices] IServiceBusLogger loggerServiceBus,
            [FromServices] ILogger<SignalRConsumerClient> loggerSignalR
        )
        {
            if (!subscribersCancels.ContainsKey(appName))
            {
                TopicConsumer<Guid, EventMessage> topicConsumer;
                var cancelSource = new CancellationTokenSource();
                var builder = new ConsumerConnectionBuilder<Guid, EventMessage>(this._appConfiguration.Kafka.CertificatePath);
                builder.WithBrokers(settings.BootstrapServers);
                builder.WithTopic(settings.Topic);
                builder.WithGroupId(settings.GroupId);
                builder.WithRetry(settings.RetryTopic, settings.Delay);
                builder.AutoOffSetReset(settings.AutoOffSetReset);
                builder.EnableAutoCommit(settings.EnableAutoCommit);
                builder.EnablePartionEof(settings.EnablePartionEof);
                //builder.MaxPollIntervalMs(settings.MaxPollIntervalMs);
                builder.WithJsonSerializer();

                var appInfo = new AppInfo
                {
                    AppName = appName,
                    CancellationToken = cancelSource,
                    SimulateError = simulateError,
                    Settings = settings
                };

                var client = new SignalRConsumerClient(appInfo, _kafkaHubContext, loggerSignalR);

                switch (settings.RetryStrategy)
                {
                    case "retry":
                        topicConsumer = TopicFactory.GetTopicConsumerWithRetry(client, builder, loggerServiceBus);
                        break;
                    case "redirect":
                        topicConsumer = TopicFactory.GetTopicConsumerWithRedirectMessage(client, builder, loggerServiceBus);
                        break;
                    default: // Only consume
                        topicConsumer = TopicFactory.GetTopicConsumer(client, builder, loggerServiceBus);
                        break;
                }

                var task = topicConsumer.Run(cancelSource.Token);

                appInfo.TaskId = task.Id;
                subscribersCancels.TryAdd(appName, appInfo);
                return Ok(appInfo);
            }

            return Ok(subscribersCancels[appName]);
        }

        [HttpGet("[action]")]
        public void UnSubscribe(string appName)
        {
            if (subscribersCancels.ContainsKey(appName))
            {
                subscribersCancels[appName].CancellationToken.Cancel();
                subscribersCancels.TryRemove(appName, out _);
            }
        }

        #region Logs

        [HttpGet("[action]")]
        public IEnumerable<LogEvent> GetAllLogs()
        {
            var logs = new List<LogEvent>();
            using (var fs = new FileStream(Constants.LOG_PATH, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {
                    var reader = new LogEventReader(sr);
                    LogEvent evt;
                    while (reader.TryRead(out evt))
                    {
                        if (evt.Properties.ContainsKey("TaskId"))
                            logs.Add(evt);
                    }
                }
            }

            return logs;
        }

        // [HttpGet("[action]")]
        // public void SubscribeLogs()
        // {
        //     var fs = new FileStream(Constants.LOG_PATH, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //     var sr = new StreamReader(fs);

        //     void exec()
        //     {
        //         var reader = new LogEventReader(sr);
        //         LogEvent evt;
        //         while (true)
        //         {
        //             if (!reader.TryRead(out evt)) continue;

        //             if (evt.Properties.ContainsKey("TaskId"))
        //                 this._logHubContext.Clients.All.SendAsync("add", evt);
        //         }
        //     }

        //     TaskHelper.SetInterval(exec, 1000);
        // }

        #endregion

    }
}
