namespace Reactive.Kafka.Bindings
{
    internal sealed class ProducerBinding : Binding
    {
        private const string onProduce = "OnProduce";
        private const string onProduceAsync = "OnProduceAsync";

        public ProducerBinding(object source, object target) : base(source, target) { }

        public void BindOnProduce()
        {
            Bind(onProduce, onProduce);
        }

        public void BindOnProduceAsync()
        {
            Bind(onProduceAsync, onProduceAsync);
        }
    }
}
