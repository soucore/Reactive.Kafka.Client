namespace Reactive.Kafka.AssemblyR
{
    public interface IAssemblyReader
    {
         IEnumerable<Type> GetConsumers();
    }
}