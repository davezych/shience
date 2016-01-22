using Shience.Test.Fakes;
using Xunit;

namespace Shience.Test
{
    public class PublisherTests
    {
        [Fact]
        public void ShienceCanInstantiateWithPublisher()
        {
            Sciencer.SetPublisher(new FakePublisher());

            var science = Sciencer.New<bool>("ShienceCanInstantiateFilePublisher");

            Assert.NotNull(science);
        }
    }
}