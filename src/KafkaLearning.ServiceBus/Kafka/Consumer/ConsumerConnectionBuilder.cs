using Confluent.Kafka;
using System;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using Confluent.Kafka.SyncOverAsync;
using KafkaLearning.ServiceBus.Kafka.Serdes;

namespace KafkaLearning.ServiceBus.Kafka.Consumer
{
    public class ConsumerConnectionBuilder<TKey, TValue>
    {
        private readonly ConsumerConfig configs;
        private readonly ConsumerBuilder<TKey, TValue> consumerBuilder;

        internal int Threads { get; private set; }
        internal string RetryTopic { get; private set; }
        internal int Delay { get; private set; }
        internal string Topic { get; private set; }
        internal string CaPath { get; private set; }

        public ConsumerConnectionBuilder(string caPath)
        {
            this.CaPath = caPath;
            this.configs = new ConsumerConfig()
            {
                MaxPollIntervalMs = 60000,
            };
Console.WriteLine("ENTROU1: " + caPath);
            if (!string.IsNullOrWhiteSpace(caPath))
            {
                Console.WriteLine("ENTROU2");
                //this.configs.Debug = "all";
                this.configs.SecurityProtocol = SecurityProtocol.Ssl;
                this.configs.SslCaLocation = caPath;
            }
            
            this.consumerBuilder = new ConsumerBuilder<TKey, TValue>(configs);
        }

        #region Fluent setters

        public ConsumerConnectionBuilder<TKey, TValue> WithGroupId(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentNullException(nameof(groupId));

            configs.GroupId = groupId;
            return this;
        }

        public ConsumerConnectionBuilder<TKey, TValue> EnableAutoCommit(bool enabled)
        {
            configs.EnableAutoCommit = enabled;
            return this;
        }

        public ConsumerConnectionBuilder<TKey, TValue> AutoOffSetReset(int autoOffSetReset) => AutoOffSetReset((AutoOffsetReset)autoOffSetReset);

        public ConsumerConnectionBuilder<TKey, TValue> AutoOffSetReset(AutoOffsetReset autoOffSetReset)
        {
            this.configs.AutoOffsetReset = autoOffSetReset;
            return this;
        }

        public ConsumerConnectionBuilder<TKey, TValue> WithBrokers(params string[] brokers)
        {
            this.configs.BootstrapServers = string.Join(",", brokers);
            return this;
        }

        public ConsumerConnectionBuilder<TKey, TValue> WithRetry(string retryTopic, int delay = 0)
        {
            this.RetryTopic = retryTopic;
            this.Delay = delay;
            return this;
        }

        // DUVIDA: Essa informação não é utilizada
        //public ConsumerConnectionBuilder<TKey, TValue> WithTopicName(string topicName)
        //{
        //    this.TopicName = topicName;
        //    return this;
        //}

        public ConsumerConnectionBuilder<TKey, TValue> EnablePartitionEof(bool? enablePartitionEof)
        {
            this.configs.EnablePartitionEof = enablePartitionEof;
            return this;
        }

        // DUVIDA: Qual seria a utilidade desse macanismo?
        //public ConsumerConnectionBuilder<TKey, TValue> WithThreadedConsumer(int threads)
        //{
        //    this.Threads = threads;
        //    return this;
        //}

        public ConsumerConnectionBuilder<TKey, TValue> MaxPollIntervalMs(int maxPollIntervalMs)
        {
            this.configs.MaxPollIntervalMs = maxPollIntervalMs;
            return this;
        }

        public ConsumerConnectionBuilder<TKey, TValue> WithSchemaRegistry(string url)
        {
            if (typeof(TKey) == typeof(string) || typeof(TValue) == typeof(string))
                return this;

            var schemaRegistryConfig = new SchemaRegistryConfig()
            {
                SchemaRegistryUrl = url
            };
            var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            var deserializerConfig = new AvroDeserializerConfig();

            consumerBuilder.SetKeyDeserializer(new AvroDeserializer<TKey>(schemaRegistryClient, deserializerConfig).AsSyncOverAsync());
            consumerBuilder.SetValueDeserializer(new AvroDeserializer<TValue>(schemaRegistryClient, deserializerConfig).AsSyncOverAsync());

            return this;
        }

        // DUVIDA: O parametro serializer não está sendo usado
        public ConsumerConnectionBuilder<TKey, TValue> WithJsonSerializer()
        {
            if (typeof(TKey) == typeof(string) || typeof(TValue) == typeof(string))
                return this;

            consumerBuilder.SetKeyDeserializer(new JsonSerializer<TKey>());
            consumerBuilder.SetValueDeserializer(new JsonSerializer<TValue>());

            return this;
        }

        public ConsumerConnectionBuilder<TKey, TValue> WithAutoCommits(IConsumerClient<TKey, TValue> consumerClient)
        {
            consumerBuilder.SetOffsetsCommittedHandler((consumer, offset) => consumerClient.AutoCommited(offset));
            return this;
        }

        public ConsumerConnectionBuilder<TKey, TValue> WithTopic(string topic)
        {
            this.Topic = topic;
            return this;
        }

        #endregion

        public IConsumer<TKey, TValue> Build()
        {
            return this.consumerBuilder.Build();
        }

        public ConsumerConfig GetConsumerConfig()
        {
            return this.configs;
        }
    }
}