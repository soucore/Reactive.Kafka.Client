using System;
using System.Reflection;

namespace Reactive.Kafka.Helpers
{
    public static class Reflection
    {
        public static void CreateDelegate(object source, EventInfo @event, object target, MethodInfo method)
        {
            if (method is not null)
            {
                Delegate @delegate = Delegate
                    .CreateDelegate(
                        @event.EventHandlerType,
                        target, method);

                @event.AddEventHandler(source, @delegate);
            }
        }
    }
}
