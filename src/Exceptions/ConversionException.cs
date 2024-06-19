namespace Reactive.Kafka.Exceptions;

public class ConversionException(string message, Exception innerException)
    : Exception(message, innerException)
{

}
