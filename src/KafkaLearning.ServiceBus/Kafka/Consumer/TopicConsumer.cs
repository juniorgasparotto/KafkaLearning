using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using KafkaLearning.ServiceBus.Extensions;
using KafkaLearning.ServiceBus.Logs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaLearning.ServiceBus.Kafka.Consumer
{
    public class TopicConsumer<TKey, TValue> : ITopicConsumer<TKey, TValue>
    {
        private readonly string topicName;
        private readonly ConsumerConfig config;
        private readonly IConsumer<TKey, TValue> consumer;
        private readonly IConsumerClient<TKey, TValue> consumerClient;
        private readonly IServiceBusLogger logger;

        // DUVIDA: é necessário obter o groupId 
        public TopicConsumer(
            string topicName,
            ConsumerConfig config,
            IConsumer<TKey, TValue> consumer,
            IConsumerClient<TKey, TValue> consumerClient,
            IServiceBusLogger logger
        )
        {
            this.topicName = topicName;
            this.config = config;
            this.consumer = consumer;
            this.consumerClient = consumerClient;
            this.logger = logger;
        }

        public Task Run(CancellationToken cancellationToken)
        {
            var groupId = this.config.GroupId;

            return Task.Run(() =>
            {
                if (cancellationToken != default)
                {
                    // Garante que ao cancelar a task, ocorra o unsubscribe, do contrário, 
                    // o cursor ficará preso na linha "c.Subscribe(...)" e o cancel() só terá efeito após retorno do kafka 
                    // que pode demorar minutos até o próximo looping do while.
                    cancellationToken.Register(() =>
                    {
                        logger.Log($"Task cancelled (Unsubscribe): Topic: {topicName}, GroupId: {groupId}");
                        consumer.Unsubscribe();
                    });
                }

                logger.Log($"Subscribe: Topic: {topicName}, GroupId: {groupId}");
                consumer.Subscribe(topicName);

                // DUVIDA: Tenho duvidas passar o consumer real pro client faz sentido na prática.
                consumerClient.Subscribed(consumer, cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumed = consumer.Consume(cancellationToken);

                        try
                        {
                            if (CanProcess(consumed, groupId))
                            {
                                logger.Log($"Event start: {consumerClient.GetType().FullName}");
                                consumerClient.MessageReceived(consumed);
                                logger.Log($"Event end: {consumerClient.GetType().FullName}");

                                // DUVIDA: Seria interessante esse código aqui?
                                // if (!config.EnableAutoCommit.Value)
                                // {
                                //     consumer.Commit();
                                //     logger.LogDebug($"Message commited: Topic: {cr.Topic}, GroupId: {_groupId}, IsPartitionEOF: {cr.IsPartitionEOF}, Key: {cr.Key}, Offset: {cr.Offset}, Partition: {cr.Partition}, Timestamp: {cr.Timestamp}");
                                // }
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Log($"Error occured: {e.Message}");
                            consumerClient.MessageReceivedExceptionOcurred(consumed, e);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        logger.Log($"Error occured: {e.Message}");
                        consumerClient.ConsumeExceptionOcurred(e);
                    }
                    catch (OperationCanceledException e)
                    {
                        logger.Log($"Error occured: {e.Message}");
                        consumer.Close();
                    }
                }
            }, cancellationToken);
        }

        private bool CanProcess(ConsumeResult<TKey, TValue> consumed, string groupId)
        {
            // DUVIDA: Pq não tinha essa verificação?
            if (consumed.IsPartitionEOF)
            {
                logger.Log($"Response consume (IsPartitionEOF = true): Topic: {consumed.Topic}, GroupId: {groupId}");
                return false;
            }

            // Caso seja o primeiro consumo ou o grupo de consumo da mensagem atual seja igual ao 
            // grupo de consumo atual, então processe a mensagem. Do contrário a mensagem não deve ser processada.
            // Esse controle é necessário para garantir que mensagens que geraram erro e voltaram para serem re-processadas 
            // SEJAM re-processadas apenas pelos grupos de consumos que geraram o erro. Os grupos de consumos que 
            // NÃO geraram erros não devem re-processar essa mensagem, pois eles não deram erro. Isso evita duplicidades.
            if (!consumed.Headers.IsFirstConsumeOrSameGroupId(groupId))
            {
                logger.Log($"Message not processed: messageGroupId ({consumed.Headers.GetOriginalGroupId()}) is not same ({groupId})");
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"Topic: {topicName}, GroupId: {config.GroupId}";
        }
    }
}
