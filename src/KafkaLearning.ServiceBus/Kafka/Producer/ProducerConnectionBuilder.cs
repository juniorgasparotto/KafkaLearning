using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Newtonsoft.Json;
using KafkaLearning.ServiceBus.Kafka.Serdes;
using System;

namespace KafkaLearning.ServiceBus.Kafka.Producer
{
    public class ProducerConnectionBuilder<TKey, TValue>
    {
        private readonly ProducerConfig producerConfig;
        private readonly ProducerBuilder<TKey, TValue> producerBuilder;

        internal bool AsyncProducer { get; private set; }

        public ProducerConnectionBuilder(string caPath)
        {
            this.producerConfig = new ProducerConfig();
            this.producerBuilder = new ProducerBuilder<TKey, TValue>(producerConfig);
            this.producerConfig.SecurityProtocol = SecurityProtocol.Plaintext;

            if (!string.IsNullOrWhiteSpace(caPath))
            {
                this.producerConfig.SecurityProtocol = SecurityProtocol.Ssl;
                this.producerConfig.SslCaLocation = caPath;
            }
        }

        #region Fluent setters

        public ProducerConnectionBuilder<TKey, TValue> WithBootstrapServers(params string[] brokers)
        {
            this.producerConfig.BootstrapServers = string.Join(",", brokers);
            return this;
        }

        public ProducerConnectionBuilder<TKey, TValue> WithSchemaRegistry(string url)
        {
            if (typeof(TKey) == typeof(string) || typeof(TValue) == typeof(string))
                return this;

            var schemaRegistryConfig = new SchemaRegistryConfig()
            {
                SchemaRegistryUrl = url
            };
            var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            var serializerConfig = new AvroSerializerConfig();

            producerBuilder.SetKeySerializer(new AvroSerializer<TKey>(schemaRegistryClient, serializerConfig));
            producerBuilder.SetValueSerializer(new AvroSerializer<TValue>(schemaRegistryClient, serializerConfig));

            return this;
        }
        public ProducerConnectionBuilder<TKey, TValue> WithJsonSerializer()
        {
            if (typeof(TKey) == typeof(string) || typeof(TValue) == typeof(string))
                return this;

            producerBuilder.SetKeySerializer(new JsonSerializer<TKey>());
            producerBuilder.SetValueSerializer(new JsonSerializer<TValue>());

            return this;
        }

        public ProducerConnectionBuilder<TKey, TValue> WithAsyncProducer()
        {
            this.AsyncProducer = true;
            return this;
        }

        #endregion

        public IProducer<TKey, TValue> Build()
        {
            return this.producerBuilder.Build();
        }
    }
}