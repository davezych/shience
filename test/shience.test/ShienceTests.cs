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

            var result = science.Test(control: (() => { return true; }), candidate: (() => { return true; })).Execute();

            Assert.Equal(true, PublishingResults.TestNamesWithResults["DefaultComparerReturnsTrueWithSameResultOnPrimitives"]);
        }

        [Fact]
        public void DefaultComparerReturnsFalseWithDifferentResultOnPrimitives()
        {
            var science = Shience.New<bool>("DefaultComparerReturnsFalseWithDifferentResultOnPrimitives");

            var result = science.Test(control: (() => { return true; }), candidate: (() => { return false; })).Execute();

            Assert.Equal(false, PublishingResults.TestNamesWithResults["DefaultComparerReturnsFalseWithDifferentResultOnPrimitives"]);
        }

        [Fact]
        public void DefaultComparerReturnsTrueWithSameResultOnObject()
        {
            var science = Shience.New<TestHelper>("DefaultComparerReturnsTrueWithSameResultOnObject");

            var result = science.Test(control: (() => { return new TestHelper { Number = 1 }; }),
                candidate: (() => { return new TestHelper { Number = 1 }; })).Execute();

            Assert.Equal(true, PublishingResults.TestNamesWithResults["DefaultComparerReturnsTrueWithSameResultOnObject"]);
        }

        [Fact]
        public void DefaultComparerReturnsFalseWithDifferentResultOnObject()
        {
            var science = Shience.New<TestHelper>("DefaultComparerReturnsFalseWithDifferentResultOnObject");

            var result = science.Test(control: (() => { return new TestHelper { Number = 1 }; }),
                candidate: (() => { return new TestHelper { Number = 2 }; })).Execute();

            Assert.Equal(false, PublishingResults.TestNamesWithResults["DefaultComparerReturnsFalseWithDifferentResultOnObject"]);
        }

        [Fact]
        public void ComparerFuncReturnsCorrectTrueResult()
        {
            var science = Shience.New<bool>("ComparerFuncReturnsCorrectTrueResult");

            var result = science.Test(control: (() => { return true; }),
                                      candidate: (() => { return true; }))
                                .WithComparer(comparer: (a, b) => { return a == b; })
                                .Execute();

            Assert.Equal(true, PublishingResults.TestNamesWithResults["ComparerFuncReturnsCorrectTrueResult"]);
        }

        [Fact]
        public void ComparerFuncReturnsCorrectFalseResult()
        {
            var science = Shience.New<bool>("ComparerFuncReturnsCorrectFalseResult");

            var result = science.Test(control: (() => { return true; }),
                                      candidate: (() => { return false; }))
                                .WithComparer(comparer: (a, b) => { return a == b; })
                                .Execute();

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

            science.Test(() => true, null).Execute();

            Assert.False(PublishingResults.TestNamesWithResults.ContainsKey("NotTestsAreRunIfCandidateIsNull"));
        }

        [Fact]
        public void ArgumentNullIsThrownIfControlIsNull()
        {
            var science = Shience.New<bool>("ArgumentNullIsThrownIfControlIsNull");

            Assert.Throws<ArgumentNullException>(() => science.Test(null, () => true).Execute());
        }

        [Fact]
        public async Task TestsAreRunInParallel()
        {
            var science = Shience.New<bool>("TestsAreRunInParallel");

            var output = string.Empty;

            var result = await science.Test(
                                            () => { Thread.Sleep(1000); output = "Control"; return true; },
                                            () => { Thread.Sleep(10); output = "Candidate"; return true; })
                                      .ExecuteAsync();

            Assert.Equal("Control", output);
        }

        private class TestHelper
        {
            public int Number { get; set; }

            public override bool Equals(object obj)
            {
                var otherTestHelper = obj as TestHelper;

                if (otherTestHelper?.Number == this.Number)
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
