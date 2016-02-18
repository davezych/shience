using System;
using Xunit;

namespace Shience.Test
{
    public sealed class ScienceTests
    {
        public sealed class New
        {
            [Fact]
            public void InstantiatingScienceThrowsArgumentNullIfTestNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => Science.New<bool>(null));
            }

            [Fact]
            public void InstantiatingScienceThrowsArgumentNullIfTestNameIsEmptyString()
            {
                Assert.Throws<ArgumentNullException>(() => Science.New<bool>(string.Empty));
            }
        }
    }
}
