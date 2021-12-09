using System;
using Reactive.Kafka.Attributes;

namespace Reactive.Kafka.Examples
{
    [ReactiveKafkaConsumer]
    public class MyConsumer {

        [ConsumerListener("MyTopic", DeserializeTo = typeof(MyMessage))]
        public void Handle(MyMessage message) {
            Console.WriteLine($"Message received: {message}");
        }
    }
    
}