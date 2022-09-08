namespace Reactive.Kafka.Bindings
{
    internal class Binding
    {
        protected readonly object source;
        protected readonly object target;

        public Binding(object source, object target)
        {
            this.source = source;
            this.target = target;
        }

        public void Bind(string @event, string method)
        {
            CreateDelegate(source, @event, target, method);
        }

        public void CreateDelegate(object source, string @event, object target, string method)
        {
            if (!string.IsNullOrEmpty(method))
            {
                var eventInfo = source.GetType().GetEvent(@event);
                var @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, target, method);

                eventInfo.AddEventHandler(source, @delegate);
            }
        }
    }
}
