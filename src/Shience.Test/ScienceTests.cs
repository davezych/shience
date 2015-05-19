using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shience.Test
{
    [TestClass]
    public class ScienceTests
    {
        [TestInitialize]
        public void ScienceTestsInitialize()
        {
            var fp = new FakePublisher();
            Shience.SetPublisher(fp);
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

        [TestMethod]
        public void ComparerFuncReturnsCorrectTrueResult()
        {
            var science = Shience.New<bool>("ComparerFuncReturnsCorrectTrueResult");

            var result = science.Test(control: (() => { return true; }),
                                      candidate: (() => { return true; }),
                                      comparer: (a, b) => { return a == b; });

            Assert.AreEqual(true, PublishingResults.TestNamesWithResults["ComparerFuncReturnsCorrectTrueResult"]);
        }

        [TestMethod]
        public void ComparerFuncReturnsCorrectFalseResult()
        {
            var science = Shience.New<bool>("ComparerFuncReturnsCorrectFalseResult");

            var result = science.Test(control: (() => { return true; }),
                                      candidate: (() => { return false; }),
                                      comparer: (a, b) => { return a == b; });

            Assert.AreEqual(false, PublishingResults.TestNamesWithResults["ComparerFuncReturnsCorrectFalseResult"]);
        }

        [TestMethod]
        public void InstantiatingScienceThrowsArgumentNullIfTestNameIsNull()
        {
            ArgumentNullException thrownException = null;

            try
            {
                var science = Shience.New<bool>(null);
            }
            catch (ArgumentNullException e)
            {
                thrownException = e;
            }

            Assert.IsNotNull(thrownException, thrownException == null ? string.Empty : thrownException.ToString());
        }

        [TestMethod]
        public void InstantiatingScienceThrowsArgumentNullIfTestNameIsEmptyString()
        {
            ArgumentNullException thrownException = null;

            try
            {
                var science = Shience.New<bool>(string.Empty);
            }
            catch (ArgumentNullException e)
            {
                thrownException = e;
            }

            Assert.IsNotNull(thrownException, thrownException == null ? string.Empty : thrownException.ToString());
        }

        [TestMethod]
        public void InstantiatingScienceThrowsArgumentNullIfPublisherIsNull()
        {
            Shience.SetPublisher(null);

            ArgumentNullException thrownException = null;

            try
            {
                var science = Shience.New<bool>(string.Empty);
            }
            catch (ArgumentNullException e)
            {
                thrownException = e;
            }

            Assert.IsNotNull(thrownException, thrownException == null ? string.Empty : thrownException.ToString());
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
