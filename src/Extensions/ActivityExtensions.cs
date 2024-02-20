using System.Diagnostics;

namespace Reactive.Kafka.Extensions;

internal static class ActivityExtensions
{
    public static Activity SetError(this Activity activity, Exception exception)
    {
        return activity?.SetError(exception.Message);
    }

    public static Activity SetError(this Activity activity, string message)
    {
        return activity?.SetStatus(ActivityStatusCode.Error, message);
    }
}
