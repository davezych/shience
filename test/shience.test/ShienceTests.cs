using System;
using Xunit;

namespace Shience.Test
{
    public sealed class ShienceTests
    {
        public sealed class New
        {
            [Fact]
            public void InstantiatingScienceThrowsArgumentNullIfTestNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => Shience.New<bool>(null, (e) => { }));
            }

            [Fact]
            public void InstantiatingScienceThrowsArgumentNullIfTestNameIsEmptyString()
            {
                Assert.Throws<ArgumentNullException>(() => Shience.New<bool>(string.Empty, (e) => { }));
            }
        }
    }
}
