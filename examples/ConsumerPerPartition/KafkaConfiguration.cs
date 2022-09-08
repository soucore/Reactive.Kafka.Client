namespace ConsumerPerPartition
{
    public class KafkaConfiguration
    {
        public string BootstrapServer { get; set; }
        public string Topic { get; set; }
        public string GroupId { get; set; }
    }
}
