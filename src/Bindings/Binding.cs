namespace Reactive.Kafka.Bindings;

internal class Binding(object source, object target)
{
    protected readonly object source = source;
    protected readonly object target = target;

    public void Bind(string @event, string method)
    {
        CreateDelegate(source, @event, target, method);
    }

    public static void CreateDelegate(object source, string @event, object target, string method)
    {
        if (!string.IsNullOrEmpty(method))
        {
            var eventInfo = source.GetType().GetEvent(@event);
            var @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, target, method);

            eventInfo.AddEventHandler(source, @delegate);
        }
    }
}
