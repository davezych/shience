using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shience.Publish;

namespace Shience.Test
{
    [TestClass]
    public class PublisherTests
    {
        [TestMethod]
        public void ShienceCanInstantiateFilePublisher()
        {
            Shience.SetPublisher(new FilePublisher(@"D:\results.txt"));
            Exception thrownException = null;

            try
            {
                var science = Shience.New<bool>("ShienceCanInstantiateFilePublisher");
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            Assert.IsNull(thrownException, thrownException != null ? thrownException.ToString() : string.Empty);
        }
    }
}
