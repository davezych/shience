using System;
using System.Threading;
using System.Threading.Tasks;
using Shience.Test.TestObjects;
using Xunit;

namespace Shience.Test
{
    public sealed class ScienceTests
    {
        public sealed class Test
        {
            [Fact]
            public void DefaultComparerReturnsTrueWithSameResultOnPrimitives()
            {
                var matched = false;
                var testName = "DefaultComparerReturnsTrueWithSameResultOnPrimitives";
                var science = Shience.New<bool>(testName, (e) =>
                {
                    matched = e.Matched;
                });

                var result = science.Test(control: () => true, candidate: () => true).Execute();

                Assert.True(matched);
            }

            [Fact]
            public void DefaultComparerReturnsFalseWithDifferentResultOnPrimitives()
            {
                var matched = false;
                var science = Shience.New<bool>("DefaultComparerReturnsFalseWithDifferentResultOnPrimitives", (e) =>
                {
                    matched = e.Matched;
                });

                var result = science.Test(control: () => true, candidate: () => false).Execute();

                Assert.False(matched);
            }

            [Fact]
            public void DefaultComparerReturnsTrueWithSameResultOnObject()
            {
                var matched = false;
                var science = Shience.New<TestNumber>("DefaultComparerReturnsTrueWithSameResultOnObject", (e) =>
                {
                    matched = e.Matched;
                });

                var result = science.Test(
                    control: () => new TestNumber(1),
                    candidate: () => new TestNumber(1)).Execute();

                Assert.True(matched);
            }

            [Fact]
            public void DefaultComparerReturnsFalseWithDifferentResultOnObject()
            {
                var matched = false;
                var science = Shience.New<TestNumber>("DefaultComparerReturnsFalseWithDifferentResultOnObject", (e) =>
                {
                    matched = e.Matched;
                });

                var result = science.Test(
                    control: () => new TestNumber(1),
                    candidate: () => new TestNumber(2)).Execute();

                Assert.False(matched);
            }

            [Fact]
            public void ComparerFuncReturnsCorrectTrueResult()
            {
                var matched = false;
                var science = Shience.New<bool>("ComparerFuncReturnsCorrectTrueResult", (e) =>
                {
                    matched = e.Matched;
                });

                var result = science
                    .Test(control: () => true, candidate: () => true)
                    .WithComparer((a, b) => a == b)
                    .Execute();

                Assert.True(matched);
            }

            [Fact]
            public void ComparerFuncReturnsCorrectFalseResult()
            {
                var matched = false;
                var science = Shience.New<bool>("ComparerFuncReturnsCorrectFalseResult", (e) =>
                {
                    matched = e.Matched;
                });

                var result = science
                    .Test(control: () => true, candidate: () => false)
                    .WithComparer((a, b) => a == b)
                    .Execute();

                Assert.False(matched);
            }

            [Fact]
            public void TestThrowsIfCandidateIsNull()
            {
                var science = Shience.New<bool>("NotTestsAreRunIfCandidateIsNull", (e) =>
                {
                });

                Assert.Throws<ArgumentNullException>(() => science.Test(() => true, null).Execute());
            }

            [Fact]
            public void ArgumentNullIsThrownIfControlIsNull()
            {
                var science = Shience.New<bool>("ArgumentNullIsThrownIfControlIsNull", (e) =>
                {
                    
                });

                Assert.Throws<ArgumentNullException>(() => science.Test(null, () => true).Execute());
            }

            [Fact]
            public void TestIsSkippedWhenWhereIsFalse()
            {
                var ran = false;
                var science = Shience.New<bool>("TestIsSkippedWhenWhereIsFalse", (e) =>
                {
                    ran = true;
                });
                var result = science
                    .Test(() => true, () => true)
                    .Where(() => false)
                    .Execute();

                Assert.True(result);
                Assert.False(ran);
            }

            [Fact]
            public void TestIsNotSkippedWhenWhereIsTrue()
            {
                var ran = false;
                var science = Shience.New<bool>("TestIsNotSkippedWhenWhereIsTrue", (e) =>
                {
                    ran = true;
                });
                var result = science
                    .Test(() => true, () => true)
                    .Where(() => true)
                    .Execute();

                Assert.True(result);
                Assert.True(ran);
            }

            [Fact]
            public void TestIsNotSkippedWhenWhereIsTrueWithMultipleChainedWheres()
            {
                var ran = false;
                var science = Shience.New<bool>("TestIsNotSkippedWhenWhereIsTrueWithMultipleChainedWheres", (e) =>
                {
                    ran = true;
                });
                var result = science
                    .Test(() => true, () => true)
                    .Where(() => true)
                    .Where(() => true)
                    .Where(() => true)
                    .Execute();

                Assert.True(result);
                Assert.True(ran);
            }

            [Fact]
            public void TestIsSkippedWhenWhereIsFalseWithMultipleChainedWheres()
            {
                var ran = false;
                var science = Shience.New<bool>("TestIsSkippedWhenWhereIsFalseWithMultipleChainedWheres", (e) =>
                {
                    ran = true;
                });
                var result = science
                    .Test(() => true, () => true)
                    .Where(() => false)
                    .Where(() => false)
                    .Where(() => false)
                    .Execute();

                Assert.True(result);
                Assert.False(ran);
            }

            [Fact]
            public void TestIsSkippedWhenMultipleChainedWheresWithMixOfTrueFalse()
            {
                var ran = false;
                var science = Shience.New<bool>("TestIsSkippedWhenMultipleChainedWheresWithMixOfTrueFalse", (e) =>
                {
                    ran = true;
                });
                var result = science
                    .Test(() => true, () => true)
                    .Where(() => false)
                    .Where(() => true)
                    .Where(() => false)
                    .Execute();

                Assert.True(result);
                Assert.False(ran);
            }

            [Fact]
            public void TestThrowsInvalidOperationExceptionIfCalledMultipleTimes()
            {
                var science = Shience.New<bool>("TestThrowsInvalidOperationExceptionIfCalledMultipleTimes", (e) => { });

                Assert.Throws<InvalidOperationException>(() => science.Test(() => true, () => true).Test(() => true, () => true).Execute());
            }

            [Fact]
            public void ExecuteThrowsMismatchExceptionIfRaiseOnMismatchCalled()
            {
                var science = Shience.New<bool>("ExecuteThrowsMismatchExceptionIfRaiseOnMismatchCalled", (e) => { });

                Assert.Throws<MismatchException>(() => science.Test(() => true, () => false).RaiseOnMismatch().Execute());
            }

            [Fact]
            public void ExecuteThrowsIfTestNotCalledFirst()
            {
                var science = Shience.New<bool>("ExecuteThrowsIfTestNotCalledFirst", (e) => { });

                Assert.Throws<InvalidOperationException>(() => science.Execute());
            }

            [Fact]
            public async Task ExecuteAsyncThrowsIfTestNotCalledFirst()
            {
                var science = Shience.New<bool>("ExecuteAsyncThrowsIfTestNotCalledFirst", (e) => { });

                await Assert.ThrowsAsync<InvalidOperationException>(() => science.ExecuteAsync());
            }
        }
        
        public sealed class TestAsync
        {
            [Fact]
            public async Task TestsAreRunInParallel()
            {
                var science = Shience.New<bool>("TestsAreRunInParallel", (e) => { });

                var output = string.Empty;

                var result = await science.Test(
                    () =>
                    {
                        Thread.Sleep(100);
                        output = "Control";
                        return true;
                    },
                    () =>
                    {
                        Thread.Sleep(10);
                        output = "Candidate";
                        return true;
                    }).ExecuteAsync();

                Assert.Equal("Control", output);
            }
        }
    }
}