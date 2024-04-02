using Reactive.Kafka.Helpers;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reactive.Kafka.Tests;

public class ActivityHelperTest
{
    [Fact]
    public void ShouldInjectCorrectlyTraceContextIntoKafkaMessage()
    {
        ActivitySource source = new("Tests");
        ActivitySource.AddActivityListener(new()
        {
            ShouldListenTo = src => src.Name == "Tests",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        });

        var activity = source.CreateActivity("setTraceHeaders-test", ActivityKind.Internal);
        var kmessage = new Message<string, string>();

        activity.Start();

        ActivityHelper.InjectTraceContext(activity, kmessage);

        var parentHeader = kmessage.Headers.FirstOrDefault(x => x.Key.Equals("traceparent"));
        var parentHeaderValue = Encoding.UTF8.GetString(parentHeader.GetValueBytes());

        Assert.NotNull(parentHeader);
        Assert.Equal(parentHeaderValue, activity.Id);
    }
}
