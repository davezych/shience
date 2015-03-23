using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shience.Publish;

namespace Shience.Test
{
    [TestClass]
    public class ScienceTests
    {
        [TestInitialize]
        public void ScienceTestsInitialize()
        {
            Shience.SetPublisher(typeof(FakePublisher<>));
        }

        [TestMethod]
        public void DefaultComparerReturnsTrueWithSameResultOnPrimitives()
        {
            var science = Shience.New<bool>("Test");

            var result = science.Test(control: (() => { return true; }), candidate: (() => { return true; }));

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void DefaultComparerReturnsFalseWithDifferentResultOnPrimitives()
        {
            var science = Shience.New<bool>("Test");

            var result = science.Test(control: (() => { return true; }), candidate: (() => { return false; }));

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void DefaultComparerReturnsTrueWithSameResultOnObject()
        {
            var science = Shience.New<TestHelper>("Test");

            var result = science.Test(control: (() => { return new TestHelper {Number = 1}; }),
                candidate: (() => { return new TestHelper {Number = 1}; }));

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void DefaultComparerReturnsFalseWithDifferentResultOnObject()
        {
            var science = Shience.New<TestHelper>("Test");

            var result = science.Test(control: (() => { return new TestHelper {Number = 1}; }),
                candidate: (() => { return new TestHelper {Number = 2}; }));

            Assert.AreEqual(true, result);
        }

        private class TestHelper
        {
            public int Number { get; set; }
        }
    }
}
