using Reactive.Kafka.Errors;
using System.Threading.Tasks;

namespace Reactive.Kafka.Interfaces
{
    public interface IKafkaConsumerError
    {
        /// <summary>
        /// Entry point for each message that couldn't
        /// be converted to desired type.
        /// </summary>
        /// <param name="sender">Kafka consumer object for analysis purpose</param>
        /// <param name="consumerError">Object with rejected message and exception stack trace</param>
        Task ConsumeError(object sender, KafkaConsumerError consumerError, Commit commit);
    }
}
