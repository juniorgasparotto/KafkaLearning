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

namespace KafkaLearning.Web.Infrastructure.ConsumersClients
{
    public class SendSignalRMessageConsumerClient : IConsumerClient<Guid, ChatMessage>
    {
        public static bool EnableTestError;

        private readonly string _roomId;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<SendSignalRMessageConsumerClient> _logger;
        private readonly ConsumerOptions _settings;
        private IConsumer<Guid, ChatMessage> consumer;

        public SendSignalRMessageConsumerClient(ConsumerOptions settings, string roomId, IHubContext<ChatHub> _hubContext, ILogger<SendSignalRMessageConsumerClient> logger)
        {
            this._roomId = roomId;
            this._hubContext = _hubContext;
            this._logger = logger;
            this._settings = settings;
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
                // não gera erros para tópicos de retry, até pq não faz sentido esses tópicos terem processors ( handlers )
                // if (EnableTestError && data.Message.ToLower().StartsWith("error") && string.IsNullOrWhiteSpace(_settings.TopicRepublish))
                if (EnableTestError && data.Message.ToLower().StartsWith("error"))
                {
                    var split = data.Message.Split('-');

                    // mecanismo para gerar erro apenas nos grupos de consumos especificios caso existam
                    // se nenhum for especificado então gera erro pra qualquer grupo de consumo
                    if (split.Length == 1 || split.Contains(this._settings.GroupId))
                        throw new Exception("Ocorreu um erro");
                }

                this._logger.LogInformation($"Handler: {this._roomId}; Message: {data.Message}");

                _hubContext.Clients.Group(this._roomId).SendAsync("chat", data);
            }
            catch
            {
                _hubContext.Clients.Group(this._roomId).SendAsync("chatError", data);
                throw;
            }
        }

    }
}
