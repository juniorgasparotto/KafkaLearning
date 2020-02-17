export const environment = {
  production: true,
  kafka: {
    consumerDefault: {
      bootstrapServers: "my-cluster-kafka-bootstrap.project-kafka:9092",
      enableAutoCommit: true,
      autoOffSetReset: 0,
      enablePartitionEof: true
    },
    producerDefault: {
      bootstrapServers: "my-cluster-kafka-bootstrap.project-kafka:9092",
      topic: "Chat"
    }
  }
};
