using System;
using Xunit;

namespace Shience.Test
{
    public sealed class ShienceTests
    {
        public sealed class SetPublisher
        {
            [Fact]
            public void InstantiatingScienceThrowsArgumentNullIfPublisherIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => Shience.SetPublisher(null));
            }
        }

        public sealed class New
        {
            [Fact]
            public void InstantiatingScienceThrowsArgumentNullIfTestNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => Shience.New<bool>(null));
            }

            [Fact]
            public void InstantiatingScienceThrowsArgumentNullIfTestNameIsEmptyString()
            {
                Assert.Throws<ArgumentNullException>(() => Shience.New<bool>(string.Empty));
            }
        }
    }
}
