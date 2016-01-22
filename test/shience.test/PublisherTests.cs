using Shience.Test.Fakes;
using Xunit;

namespace Shience.Test
{
    public class PublisherTests
    {
        [Fact]
        public void ShienceCanInstantiateWithPublisher()
        {
            Shience.SetPublisher(new FakePublisher());

            var science = Shience.New<bool>("ShienceCanInstantiateFilePublisher");

            Assert.NotNull(science);
        }
    }
}