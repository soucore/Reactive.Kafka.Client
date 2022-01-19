namespace ConsumerPerPartition
{
    internal record class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
