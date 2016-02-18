using Xunit;

namespace Shience.Test
{
    public sealed class PublisherTests
    {
        [Fact]
        public void ShienceCanInstantiateWithPublisher()
        {
            var science = Science.New<bool>("ShienceCanInstantiateFilePublisher");
            
            Assert.NotNull(science);
        }
    }
}