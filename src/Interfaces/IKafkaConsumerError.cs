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
        /// <param name="consumerError">Object containing error information</param>
        /// <param name="commit">Offset commit function</param>
        /// <returns></returns>
        Task OnConsumeError(KafkaConsumerError consumerError, Commit commit);
    }
}
