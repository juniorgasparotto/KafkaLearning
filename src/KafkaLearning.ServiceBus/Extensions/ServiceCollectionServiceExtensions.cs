using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KafkaLearning.ServiceBus.HostedServices;
using KafkaLearning.ServiceBus.Kafka.Consumer;
using KafkaLearning.ServiceBus.Kafka.Producer;
using KafkaLearning.ServiceBus.Logs;
using KafkaLearning.ServiceBus.Helpers;

namespace KafkaLearning.ServiceBus.Extensions
{
    public static class ServiceCollectionServiceExtensions
    {
        public static KafkaBuilder AddKafka(this IServiceCollection services)
        {
            // Injeta as classes que são transversais
            services.AddSingleton<IServiceBusLogger, ServiceBusLogger>();
            return new KafkaBuilder(services);
        }

        public static KafkaBuilder AddProducer<TKey, TValue>(
            this KafkaBuilder kafkaBuilder, 
            Action<IServiceProvider, ProducerConnectionBuilder<TKey, TValue>> optionsAction
        )
        {
            return AddProducer<NullProducerClient<TKey, TValue>, TKey, TValue>(kafkaBuilder, optionsAction);
        }

        public static KafkaBuilder AddProducer<TProducerClient, TKey, TValue>(
            this KafkaBuilder kafkaBuilder, 
            Action<IServiceProvider, ProducerConnectionBuilder<TKey, TValue>> optionsAction
        )
            where TProducerClient : class, IProducerClient<TKey, TValue>
        {
            var services = kafkaBuilder.Services;
            
            services.AddSingleton<IProducerClient<TKey, TValue>, TProducerClient>();

            services.AddSingleton<IProducerSender<TKey, TValue>>(s =>
            {
                var logger = s.GetRequiredService<IServiceBusLogger>();
                var builder = new ProducerConnectionBuilder<TKey, TValue>();
                var producerClient = s.GetRequiredService<IProducerClient<TKey, TValue>>();

                optionsAction(s, builder);

                if (builder.AsyncProducer)
                    return new ProducerAsyncSender<TKey, TValue>(
                        builder.Build(),
                        producerClient,
                        logger
                    );
                else
                    return new ProducerSyncSender<TKey, TValue>(
                        builder.Build(),
                        producerClient,
                        logger
                    );
            });

            return kafkaBuilder;
        }

        public static KafkaBuilder AddConsumer<TConsumerClient, TKey, TValue>(
            this KafkaBuilder kafkaBuilder,
            Action<IServiceProvider, ConsumerConnectionBuilder<TKey, TValue>> optionsAction
        )
            where TConsumerClient : class, IConsumerClient<TKey, TValue>
        {
            var services = kafkaBuilder.Services;

            services.AddSingleton<TConsumerClient>();

            services.AddSingleton<IHostedService>(s =>
            {
                var logger = s.GetService<IServiceBusLogger>();
                var loggerServiceHosted = s.GetService<ILogger<DefaultConsumerHostedService<TKey, TValue>>>();
                var client = s.GetRequiredService<TConsumerClient>();
                var cb = new ConsumerConnectionBuilder<TKey, TValue>();

                optionsAction(s, cb);

                var topicConsumer = TopicFactory.GetTopicConsumer(client, cb, logger);
                return new DefaultConsumerHostedService<TKey, TValue>(topicConsumer, loggerServiceHosted);
            });

            return kafkaBuilder;
        }

        public static KafkaBuilder AddConsumerWithRetry<TConsumerClient, TKey, TValue>(
            this KafkaBuilder kafkaBuilder,
            Action<IServiceProvider, ConsumerConnectionBuilder<TKey, TValue>> optionsAction
        )
            where TConsumerClient : class, IConsumerClient<TKey, TValue>
        {
            var services = kafkaBuilder.Services;

            services.AddSingleton<TConsumerClient>();

            services.AddTransient<IHostedService>((s) =>
            {
                var logger = s.GetService<IServiceBusLogger>();
                var loggerServiceHosted = s.GetService<ILogger<DefaultConsumerHostedService<TKey, TValue>>>();
                var client = s.GetRequiredService<TConsumerClient>();
                var cb = new ConsumerConnectionBuilder<TKey, TValue>();

                optionsAction(s, cb);

                var topicConsumer = TopicFactory.GetTopicConsumerWithRetry(client, cb, logger);
                return new DefaultConsumerHostedService<TKey, TValue>(topicConsumer, loggerServiceHosted);
            });

            return kafkaBuilder;
        }

        /// <summary>
        /// Esse método apenas redireciona uma mensagem para outro tópico.
        /// </summary>
        public static KafkaBuilder AddConsumerWithRedirectMessage<TConsumerClient, TKey, TValue>(
            this KafkaBuilder kafkaBuilder,
            Action<IServiceProvider, ConsumerConnectionBuilder<TKey, TValue>> optionsAction
        )
            where TConsumerClient : class, IConsumerClient<TKey, TValue>
        {
            var services = kafkaBuilder.Services;

            services.AddSingleton<TConsumerClient>();

            services.AddTransient<IHostedService>((s) =>
            {
                var logger = s.GetService<IServiceBusLogger>();
                var loggerServiceHosted = s.GetService<ILogger<DefaultConsumerHostedService<TKey, TValue>>>();
                var client = s.GetRequiredService<TConsumerClient>();
                var cb = new ConsumerConnectionBuilder<TKey, TValue>();

                optionsAction(s, cb);

                var topicConsumer = TopicFactory.GetTopicConsumerWithRedirectMessage(client, cb, logger);
                return new DefaultConsumerHostedService<TKey, TValue>(topicConsumer, loggerServiceHosted);
            });

            return kafkaBuilder;
        }
    }
}
