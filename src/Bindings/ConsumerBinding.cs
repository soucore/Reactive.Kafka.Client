namespace Reactive.Kafka.Bindings
{
    internal sealed class ConsumerBinding : Binding
    {
        private const string onConsumeEvent = "OnConsume";
        private const string onBeforeSerialization = "OnBeforeSerialization";
        private const string onAfterSerialization = "OnAfterSerialization";
        private const string onConsumeError = "OnConsumeError";

        public ConsumerBinding(object source, object target) : base(source, target) { }

        public void BindOnConsume()
        {
            Bind(onConsumeEvent, onConsumeEvent);
        }

        public void BindOnBeforeSerialization()
        {
            if (target is IKafkaSerialization)
                Bind(onBeforeSerialization, onBeforeSerialization);
        }

        public void BindOnAfterSerialization()
        {
            if (target is IKafkaSerialization)
                Bind(onAfterSerialization, onAfterSerialization);
        }

        public void BindOnConsumeError()
        {
            if (target is IKafkaConsumerError)
                Bind(onConsumeError, onConsumeError);
        }
    }
}
