using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using KafkaLearning.ServiceBus.Configurations;
using KafkaLearning.ServiceBus.Kafka.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KafkaLearning.Web.Core.Entities;
using KafkaLearning.Web.Hubs;
using KafkaLearning.Web.Infrastructure.ViewModel;

namespace KafkaLearning.Web.Infrastructure.ConsumersClients
{
    public class SendSignalRMessageConsumerClient : IConsumerClient<Guid, ChatMessage>
    {
        private readonly string _roomId;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<SendSignalRMessageConsumerClient> _logger;
        private IConsumer<Guid, ChatMessage> consumer;
        private AppInfo _appInfo;

        public SendSignalRMessageConsumerClient(AppInfo appInfo, IHubContext<ChatHub> _hubContext, ILogger<SendSignalRMessageConsumerClient> logger)
        {
            this._hubContext = _hubContext;
            this._logger = logger;
            this._appInfo = appInfo;
        }

        public void Subscribed(IConsumer<Guid, ChatMessage> consumer, CancellationToken cancellationToken)
        {
            this.consumer = consumer;
        }

        public void MessageReceived(ConsumeResult<Guid, ChatMessage> result)
        {
            this.SubscribeInternal(result.Message.Value);
        }

        public void AutoCommited(CommittedOffsets offsets)
        {
            //throw new NotImplementedException();
        }

        public void Commited(IEnumerable<TopicPartitionOffset> offsets)
        {
            //throw new NotImplementedException();
        }

        public void ConsumeExceptionOcurred(ConsumeException e)
        {
            //throw new NotImplementedException();
        }

        public void MessageReceivedExceptionOcurred(ConsumeResult<Guid, ChatMessage> result, Exception e)
        {
            //throw new NotImplementedException();
        }

        private void SubscribeInternal(ChatMessage data)
        {
            data.ReceiveDate = DateTime.Now;

            try
            {
                if (this._appInfo.SimulateError)
                    throw new Exception("An error has occurred");
                _hubContext.Clients.Group(this._appInfo.AppName).SendAsync("chat", data);
            }
            catch
            {
                _hubContext.Clients.Group(this._appInfo.AppName).SendAsync("chatError", data);
                throw;
            }
        }

    }
}
