namespace Reactive.Kafka.Helpers
{
    public static class Reflection
    {
        public static void CreateDelegate(object source, string @event, object target, string method)
        {
            if (!string.IsNullOrEmpty(method))
            {
                EventInfo eventInfo = source.GetType().GetEvent(@event);

                var @delegate = Delegate
                    .CreateDelegate(eventInfo.EventHandlerType, target, method);

                eventInfo.AddEventHandler(source, @delegate);
            }
        }
    }
}
