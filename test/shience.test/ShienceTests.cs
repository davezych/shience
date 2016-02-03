using System;
using System.Threading;
using System.Threading.Tasks;
using Shience.Test.Fakes;
using Xunit;

namespace Shience.Test
{
    public class ShienceTests
    {
        public ShienceTests()
        {
            var fp = new FakePublisher();
            Shience.SetPublisher(fp);
        }

        [Fact]
        public void DefaultComparerReturnsTrueWithSameResultOnPrimitives()
        {
            var science = Shience.New<bool>("DefaultComparerReturnsTrueWithSameResultOnPrimitives");

            var result = science.Test(control: () => true, candidate: () => true);

            Assert.Equal(true, PublishingResults.TestNamesWithResults["DefaultComparerReturnsTrueWithSameResultOnPrimitives"]);
        }

        [Fact]
        public void DefaultComparerReturnsFalseWithDifferentResultOnPrimitives()
        {
            var science = Shience.New<bool>("DefaultComparerReturnsFalseWithDifferentResultOnPrimitives");

            var result = science.Test(control: () => true, candidate: () => false);

            Assert.Equal(false, PublishingResults.TestNamesWithResults["DefaultComparerReturnsFalseWithDifferentResultOnPrimitives"]);
        }

        [Fact]
        public void DefaultComparerReturnsTrueWithSameResultOnObject()
        {
            var science = Shience.New<TestNumber>("DefaultComparerReturnsTrueWithSameResultOnObject");

            var result = science.Test(
                control: () => new TestNumber(1),
                candidate: () => new TestNumber(1));

            Assert.Equal(true, PublishingResults.TestNamesWithResults["DefaultComparerReturnsTrueWithSameResultOnObject"]);
        }

        [Fact]
        public void DefaultComparerReturnsFalseWithDifferentResultOnObject()
        {
            var science = Shience.New<TestNumber>("DefaultComparerReturnsFalseWithDifferentResultOnObject");

            var result = science.Test(
                control: () => new TestNumber(1),
                candidate: () => new TestNumber(2));

            Assert.Equal(false, PublishingResults.TestNamesWithResults["DefaultComparerReturnsFalseWithDifferentResultOnObject"]);
        }

        [Fact]
        public void ComparerFuncReturnsCorrectTrueResult()
        {
            var science = Shience.New<bool>("ComparerFuncReturnsCorrectTrueResult");

            var result = science.Test(control: () => true,
                                      candidate: () => true,
                                      comparer: (a, b) => a == b);

            Assert.Equal(true, PublishingResults.TestNamesWithResults["ComparerFuncReturnsCorrectTrueResult"]);
        }

        [Fact]
        public void ComparerFuncReturnsCorrectFalseResult()
        {
            var science = Shience.New<bool>("ComparerFuncReturnsCorrectFalseResult");

            var result = science.Test(control: () => true,
                                      candidate: () => false,
                                      comparer: (a, b) => a == b);

            Assert.Equal(false, PublishingResults.TestNamesWithResults["ComparerFuncReturnsCorrectFalseResult"]);
        }

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

        [Fact]
        public void InstantiatingScienceThrowsArgumentNullIfPublisherIsNull()
        {
            Shience.SetPublisher(null);

            Assert.Throws<ArgumentNullException>(() => Shience.New<bool>("InstantiatingScienceThrowsArgumentNullIfPublisherIsNull"));
        }

        [Fact]
        public void NoTestsAreRunIfCandidateIsNull()
        {
            var science = Shience.New<bool>("NotTestsAreRunIfCandidateIsNull");

            science.Test(() => true, null);

            Assert.False(PublishingResults.TestNamesWithResults.ContainsKey("NotTestsAreRunIfCandidateIsNull"));
        }

        [Fact]
        public void ArgumentNullIsThrownIfControlIsNull()
        {
            var science = Shience.New<bool>("ArgumentNullIsThrownIfControlIsNull");

            Assert.Throws<ArgumentNullException>(() => science.Test(null, () => true));
        }

        [Fact]
        public async Task TestsAreRunInParallel()
        {
            var science = Shience.New<bool>("TestsAreRunInParallel");

            var output = string.Empty;

            var result = await science.TestAsync(() => { Thread.Sleep(1000); output = "Control"; return true; },
                () => { Thread.Sleep(10); output = "Candidate"; return true; });

            Assert.Equal("Control", output);
        }

        internal sealed class TestNumber
        {
            public TestNumber(int number)
            {
                Number = number;
            }

            private int Number { get; }

            public override bool Equals(object obj)
            {
                var otherTestHelper = obj as TestNumber;

                return otherTestHelper?.Number == Number;
            }

            public override int GetHashCode()
            {
                return Number.GetHashCode();
            }
        }
    }
}
