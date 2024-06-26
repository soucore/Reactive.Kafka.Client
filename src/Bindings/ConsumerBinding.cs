﻿namespace Reactive.Kafka.Bindings;

internal sealed class ConsumerBinding(object source, object target) : Binding(source, target)
{
    private const string onConsume = "OnConsume";
    private const string onBeforeSerialization = "OnBeforeSerialization";
    private const string onAfterSerialization = "OnAfterSerialization";
    private const string onConsumeError = "OnConsumeError";

    public void BindOnConsume()
    {
        Bind(onConsume, onConsume);
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
