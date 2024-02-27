namespace Reactive.Kafka.Extensions;

internal static class ActivityExtensions
{
    public static Activity SetError(this Activity activity, Exception exception, in TagList tags = default)
    {
        activity?.RecordException(exception, tags);
        activity?.SetStatus(ActivityStatusCode.Error);

        return activity;
    }
}
