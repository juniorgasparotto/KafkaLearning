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

namespace KafkaLearning.Web.Controllers
{
    [Route("api/[controller]")]
    public partial class ChatController : Controller
    {
        private static ConcurrentDictionary<string, AppInfo> subscribersCancels = new ConcurrentDictionary<string, AppInfo>();

        private readonly IHubContext<ChatHub> _kafkaHubContext;
        private readonly IHubContext<LogHub> _logHubContext;
        private readonly AppConfigurationOptions _appConfiguration;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            ILogger<ChatController> logger,
            IOptions<AppConfigurationOptions> appConfiguration,
            IHubContext<ChatHub> kafkaHubContext,
            IHubContext<LogHub> logHubContext
        )
        {
            this._kafkaHubContext = kafkaHubContext;
            this._logHubContext = logHubContext;
            this._appConfiguration = appConfiguration.Value;
            this._logger = logger;
        }


        [HttpGet("[action]")]
        public void EnableError(bool enable)
        {
            SendSignalRMessageConsumerClient.EnableTestError = enable;
        }

        [HttpGet("[action]")]
        public void UnSubscribeAll()
        {
            foreach (var c in subscribersCancels)
                c.Value.CancellationToken.Cancel();
            subscribersCancels.Clear();
        }

        [HttpGet("[action]")]
        public IEnumerable<string> GetAllSubscribers()
        {
            return subscribersCancels.Keys;
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
            var builder = new ProducerConnectionBuilder<Guid, ChatMessage>();
            var producer = builder
                            .WithBootstrapServers(request.Settings.BootstrapServers)
                            .WithAsyncProducer()
                            .WithJsonSerializer()
                            .Build();
            var producerSender = new ProducerAsyncSender<Guid, ChatMessage>(
                producer,
                null,
                logger
            );

            await producerSender.SendAsync(request.Message.Id, request.Message, request.Settings.Topic);
        }

        [HttpPost("[action]")]
        public IActionResult Subscribe(
            string roomId,
            [FromBody]ConsumerOptions settings,
            [FromServices] IServiceBusLogger loggerServiceBus,
            [FromServices] ILogger<SendSignalRMessageConsumerClient> loggerSignalR
        )
        {
            if (!subscribersCancels.ContainsKey(roomId))
            {
                TopicConsumer<Guid, ChatMessage> topicConsumer;
                var cancelSource = new CancellationTokenSource();
                var builder = new ConsumerConnectionBuilder<Guid, ChatMessage>();
                builder.WithBrokers(settings.BootstrapServers);
                builder.WithTopic(settings.Topic);
                builder.WithGroupId(settings.GroupId);
                builder.WithRetry(settings.RetryTopic, settings.Delay);
                builder.AutoOffSetReset(settings.AutoOffSetReset);
                builder.EnableAutoCommit(settings.EnableAutoCommit);
                builder.EnablePartionEof(settings.EnablePartionEof);
                //builder.MaxPollIntervalMs(settings.MaxPollIntervalMs);
                builder.WithJsonSerializer();

                var client = new SendSignalRMessageConsumerClient(settings, roomId, _kafkaHubContext, loggerSignalR);

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
                var appInfo = new AppInfo
                {
                    AppName = roomId,
                    CancellationToken = cancelSource,
                    TaskId = task.Id
                };

                subscribersCancels.TryAdd(roomId, appInfo);
                return Ok(appInfo);
            }

            return Ok(subscribersCancels[roomId]);
        }

        [HttpGet("[action]")]
        public void UnSubscribe(string roomId)
        {
            if (subscribersCancels.ContainsKey(roomId))
            {
                subscribersCancels[roomId].CancellationToken.Cancel();
                subscribersCancels.TryRemove(roomId, out _);
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
