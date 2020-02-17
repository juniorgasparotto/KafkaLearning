export const environment = {
  production: true,
  kafka: {
    consumerDefault: {
      bootstrapServers: "my-cluster-kafka-bootstrap-project-kafka.192.168.0.10.nip.io:443",
      enableAutoCommit: true,
      autoOffSetReset: 0,
      enablePartitionEof: true
    },
    producerDefault: {
      bootstrapServers: "my-cluster-kafka-bootstrap:9092",
      topic: "Chat"
    }
  }
};
