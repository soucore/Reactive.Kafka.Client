namespace Reactive.Kafka.Bindings;

internal sealed class ProducerBinding(object source, object target) : Binding(source, target)
{
    private const string onProduce = "OnProduce";
    private const string onProduceAsync = "OnProduceAsync";

    public void BindOnProduce()
    {
        Bind(onProduce, onProduce);
    }

    public void BindOnProduceAsync()
    {
        Bind(onProduceAsync, onProduceAsync);
    }
}
