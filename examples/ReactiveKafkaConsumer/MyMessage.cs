namespace Reactive.Kafka.Examples
{
    public class MyMessage
    {
        public long OrderTime { get; set; }
        public int OrderId { get; set; }
        public string? ItemId { get; set; }
        public Address? Address { get; set; }

        public override string ToString()
        {
            return $"OrderTime: {OrderTime}; OrderId: {OrderId}; ItemId: {ItemId}; Address: ({Address})";
        }
    }

    public class Address
    {
        public string? City { get; set; }
        public string? State { get; set; }
        public int ZipCode { get; set; }

        public override string ToString()
        {
            return $"City: {City}; State: {State}; ZipCode: {ZipCode.ToString()}";
        }
    }
}