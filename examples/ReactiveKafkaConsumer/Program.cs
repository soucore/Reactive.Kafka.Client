using System;
using Confluent.Kafka;
using Reactive.Kafka.Client;

var consumerConfig = new ConsumerConfig
    {
        BootstrapServers = "MyServers",
        SaslUsername = "MyUser",
        SaslPassword = "MyPassword",
        AutoOffsetReset = AutoOffsetReset.Latest,
        ClientId = Guid.NewGuid().ToString(),
        SecurityProtocol = SecurityProtocol.SaslSsl,
        SaslMechanism = SaslMechanism.Plain        
    };

ReactiveKafkaClient.Run(client =>
    client
        .UseConsumer(consumerConfig)
);