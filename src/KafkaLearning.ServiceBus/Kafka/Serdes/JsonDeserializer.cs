using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace KafkaLearning.ServiceBus.Kafka.Serdes
{
    public class JsonSerializer<T> : IDeserializer<T>, ISerializer<T>
    {
        private readonly JsonSerializer serializer;

        public JsonSerializer()
        {
            serializer = new JsonSerializer();
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
                return default(T);

            using (var r = new StringReader(Encoding.UTF8.GetString(data.ToArray())))
            using (var jr = new JsonTextReader(r))
                return serializer.Deserialize<T>(jr);
        }

        public byte[] Serialize(T data, SerializationContext context)
        {
            var sb = new StringBuilder();
            using (var w = new StringWriter(sb))
                serializer.Serialize(w, data);
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}