using Xunit;

namespace Reactive.Kafka.Tests
{
    public class ConsumerMessageTest
    {
        [Fact]
        public void ConsumerMessageInteger()
        {
            // Arrange
            ConsumerMessage<int> message1 = new("key1", 1);
            ConsumerMessage<int> message2 = new("key2", 2);

            // Assert
            Assert.Equal("key1", message1.Key);
            Assert.Equal("key2", message2.Key);
            Assert.Equal(1, message1.Message);
            Assert.Equal(2, message2.Message);
        }
        
        [Fact]
        public void ConsumerMessageObject()
        {
            // Arrange
            ConsumerMessage<Person> message1 = new("key1", new Person(18, "Anne"));
            ConsumerMessage<Person> message2 = new("key2", new Person(40, "John"));

            // Assert
            Assert.Equal("key1", message1.Key);
            Assert.Equal("key2", message2.Key);
            Assert.Equal(18, message1.Message.Age);
            Assert.Equal(40, message2.Message.Age);
            Assert.Equal("Anne", message1.Message.Name);
            Assert.Equal("John", message2.Message.Name);
        }

        public record Person(int Age, string Name);
    }
}
