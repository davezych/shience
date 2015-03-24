using System;
using System.Collections;
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
            var science = Shience.New<bool>("DefaultComparerReturnsTrueWithSameResultOnPrimitives");

            var result = science.Test(control: (() => { return true; }), candidate: (() => { return true; }));

            Assert.AreEqual(true, PublishingResults.TestNamesWithResults["DefaultComparerReturnsTrueWithSameResultOnPrimitives"]);
        }

        [TestMethod]
        public void DefaultComparerReturnsFalseWithDifferentResultOnPrimitives()
        {
            var science = Shience.New<bool>("DefaultComparerReturnsFalseWithDifferentResultOnPrimitives");

            var result = science.Test(control: (() => { return true; }), candidate: (() => { return false; }));

            Assert.AreEqual(false, PublishingResults.TestNamesWithResults["DefaultComparerReturnsFalseWithDifferentResultOnPrimitives"]);
        }

        [TestMethod]
        public void DefaultComparerReturnsTrueWithSameResultOnObject()
        {
            var science = Shience.New<TestHelper>("DefaultComparerReturnsTrueWithSameResultOnObject");

            var result = science.Test(control: (() => { return new TestHelper {Number = 1}; }),
                candidate: (() => { return new TestHelper {Number = 1}; }));

            Assert.AreEqual(true, PublishingResults.TestNamesWithResults["DefaultComparerReturnsTrueWithSameResultOnObject"]);
        }

        [TestMethod]
        public void DefaultComparerReturnsFalseWithDifferentResultOnObject()
        {
            var science = Shience.New<TestHelper>("DefaultComparerReturnsFalseWithDifferentResultOnObject");

            var result = science.Test(control: (() => { return new TestHelper {Number = 1}; }),
                candidate: (() => { return new TestHelper {Number = 2}; }));

            Assert.AreEqual(false, PublishingResults.TestNamesWithResults["DefaultComparerReturnsFalseWithDifferentResultOnObject"]);
        }

        private class TestHelper
        {
            public int Number { get; set; }

            public override bool Equals(object obj)
            {
                var otherTestHelper = obj as TestHelper;
                if (otherTestHelper == null)
                {
                    return false;
                }

                if (otherTestHelper.Number == this.Number)
                {
                    return true;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode() ^ Number;
            }
        }
    }
}
