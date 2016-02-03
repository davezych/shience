using System;
using System.Threading;
using System.Threading.Tasks;
using Shience.Test.Fakes;
using Shience.Test.TestObjects;
using Xunit;

namespace Shience.Test
{
    public sealed class ScienceTests
    {
        public sealed class Test
        {
            public Test()
            {
                var fp = new FakePublisher();
                Shience.SetPublisher(fp);
            }
            
            [Fact]
            public void DefaultComparerReturnsTrueWithSameResultOnPrimitives()
            {
                var science = Shience.New<bool>("DefaultComparerReturnsTrueWithSameResultOnPrimitives");

                var result = science.Test(control: () => true, candidate: () => true).Execute();

                Assert.Equal(true, PublishingResults.TestNamesWithResults["DefaultComparerReturnsTrueWithSameResultOnPrimitives"]);
            }

            [Fact]
            public void DefaultComparerReturnsFalseWithDifferentResultOnPrimitives()
            {
                var science = Shience.New<bool>("DefaultComparerReturnsFalseWithDifferentResultOnPrimitives");

                var result = science.Test(control: () => true, candidate: () => false).Execute();

                Assert.Equal(false, PublishingResults.TestNamesWithResults["DefaultComparerReturnsFalseWithDifferentResultOnPrimitives"]);
            }

            [Fact]
            public void DefaultComparerReturnsTrueWithSameResultOnObject()
            {
                var science = Shience.New<TestNumber>("DefaultComparerReturnsTrueWithSameResultOnObject");

                var result = science.Test(
                    control: () => new TestNumber(1),
                    candidate: () => new TestNumber(1)).Execute();

                Assert.Equal(true, PublishingResults.TestNamesWithResults["DefaultComparerReturnsTrueWithSameResultOnObject"]);
            }

            [Fact]
            public void DefaultComparerReturnsFalseWithDifferentResultOnObject()
            {
                var science = Shience.New<TestNumber>("DefaultComparerReturnsFalseWithDifferentResultOnObject");

                var result = science.Test(
                    control: () => new TestNumber(1),
                    candidate: () => new TestNumber(2)).Execute();

                Assert.Equal(false, PublishingResults.TestNamesWithResults["DefaultComparerReturnsFalseWithDifferentResultOnObject"]);
            }

            [Fact]
            public void ComparerFuncReturnsCorrectTrueResult()
            {
                var science = Shience.New<bool>("ComparerFuncReturnsCorrectTrueResult");

                var result = science.Test(control: () => true,
                    candidate: () => true)
                    .WithComparer((a, b) => a == b)
                    .Execute();

                Assert.Equal(true, PublishingResults.TestNamesWithResults["ComparerFuncReturnsCorrectTrueResult"]);
            }

            [Fact]
            public void ComparerFuncReturnsCorrectFalseResult()
            {
                var science = Shience.New<bool>("ComparerFuncReturnsCorrectFalseResult");

                var result = science.Test(control: () => true,
                    candidate: () => false,
                    comparer: (a, b) => a == b).Execute();

                Assert.Equal(false, PublishingResults.TestNamesWithResults["ComparerFuncReturnsCorrectFalseResult"]);
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

        }
        
        public sealed class TestAsync
        {
            public TestAsync()
            {
                var fp = new FakePublisher();
                Shience.SetPublisher(fp);
            }
            
            [Fact]
            public async Task TestsAreRunInParallel()
            {
                var science = Shience.New<bool>("TestsAreRunInParallel");

                var output = string.Empty;

                var result = await science.TestAsync(
                    () =>
                    {
                        Thread.Sleep(1000);
                        output = "Control";
                        return true;
                    },
                    () =>
                    {
                        Thread.Sleep(10);
                        output = "Candidate";
                        return true;
                    }).Execute();

                Assert.Equal("Control", output);
            }
        }
    }
}