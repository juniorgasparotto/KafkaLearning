using KafkaLearning.ServiceBus.Kafka.Consumer;
using KafkaLearning.ServiceBus.Kafka.Producer;
using KafkaLearning.ServiceBus.Logs;

namespace KafkaLearning.ServiceBus.Helpers
{
    public static class TopicFactory
    {
        public static TopicConsumer<TKey, TValue> GetTopicConsumer<TKey, TValue>(
            IConsumerClient<TKey, TValue> client,
            ConsumerConnectionBuilder<TKey, TValue> cb,
            IServiceBusLogger logger
        )
        {
            var consumer = cb.Build();

            var topicConsumer = new TopicConsumer<TKey, TValue>(
                cb.Topic,
                cb.GetConsumerConfig(),
                consumer,
                client,
                logger
            );

            return topicConsumer;
        }

        public static TopicConsumer<TKey, TValue> GetTopicConsumerWithRetry<TKey, TValue>(
            IConsumerClient<TKey, TValue> client,
            ConsumerConnectionBuilder<TKey, TValue> cb,
            IServiceBusLogger logger
        )
        {
            var producerSender = GetResendProducerSender(cb, logger);

            // Cria o consumidor
            var consumer = cb.Build();
            var consumerConfig = cb.GetConsumerConfig();

            var retryConsumerClient = new RetryConsumerClient<TKey, TValue>(
                cb.RetryTopic,
                consumerConfig.GroupId, // Deve ser, obrigatoriamente, o mesmo groupId do tópico original
                cb.Delay,
                client,
                producerSender,
                logger
            );

            return new TopicConsumer<TKey, TValue>(
                cb.Topic,
                consumerConfig,
                consumer,
                retryConsumerClient,
                logger
            );
        }

        /// <summary>
        /// Esse método apenas redireciona uma mensagem para outro tópico.
        /// </summary>
        public static TopicConsumer<TKey, TValue> GetTopicConsumerWithRedirectMessage<TKey, TValue>(
            IConsumerClient<TKey, TValue> client,
            ConsumerConnectionBuilder<TKey, TValue> cb,
            IServiceBusLogger logger
        )
        {
            var producerSender = GetResendProducerSender(cb, logger);

            // Cria o consumidor
            var consumer = cb.Build();
            var consumerConfig = cb.GetConsumerConfig();

            var redirect = new RedirectConsumerClient<TKey, TValue>(
                cb.RetryTopic,
                cb.Delay,
                client,
                producerSender,
                logger
            );

            return new TopicConsumer<TKey, TValue>(
                cb.Topic,
                consumerConfig,
                consumer,
                redirect,
                logger
            );
        }

        private static ProducerSyncSender<TKey, TValue> GetResendProducerSender<TKey, TValue>(ConsumerConnectionBuilder<TKey, TValue> cb, IServiceBusLogger logger)
        {
            // Cria o produtor com o mesmo endereço do consumidor (isso pode melhorar)
            var produtorBuilder = new ProducerConnectionBuilder<TKey, TValue>();
            var producer = produtorBuilder
                .WithBootstrapServers(cb.GetConsumerConfig().BootstrapServers)
                .WithAsyncProducer()
                .WithJsonSerializer()
                .Build();

            var producerSender = new ProducerSyncSender<TKey, TValue>(producer, new NullProducerClient<TKey, TValue>(logger), logger);
            return producerSender;
        }
    }
}
