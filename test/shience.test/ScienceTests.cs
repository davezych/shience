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
            [Fact]
            public void DefaultComparerReturnsTrueWithSameResultOnPrimitives()
            {
                var matched = false;
                var testName = "DefaultComparerReturnsTrueWithSameResultOnPrimitives";

                var result = Shience.New<bool>(testName)
                    .Test(control: () => true, candidate: () => true)
                    .PublishTo(r => matched = r.Matched)
                    .Execute();
                
                Assert.True(matched);
            }

            [Fact]
            public void DefaultComparerReturnsFalseWithDifferentResultOnPrimitives()
            {
                var matched = false;
                var result = Shience.New<bool>("DefaultComparerReturnsFalseWithDifferentResultOnPrimitives")
                    .Test(control: () => true, candidate: () => false)
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.False(matched);
            }

            [Fact]
            public void DefaultComparerReturnsTrueWithSameResultOnObject()
            {
                var matched = false;
                var result = Shience.New<TestNumber>("DefaultComparerReturnsTrueWithSameResultOnObject")
                    .Test(control: () => new TestNumber(1), candidate: () => new TestNumber(1))
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.True(matched);
            }

            [Fact]
            public void DefaultComparerReturnsFalseWithDifferentResultOnObject()
            {
                var matched = false;
                var result = Shience.New<TestNumber>("DefaultComparerReturnsFalseWithDifferentResultOnObject")
                    .Test(control: () => new TestNumber(1), candidate: () => new TestNumber(2))
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.False(matched);
            }

            [Fact]
            public void ComparerFuncReturnsCorrectTrueResult()
            {
                var matched = false;
                var result = Shience.New<bool>("ComparerFuncReturnsCorrectTrueResult")
                    .Test(control: () => true, candidate: () => true)
                    .WithComparer((a, b) => a == b)
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.True(matched);
            }

            [Fact]
            public void ComparerFuncReturnsCorrectFalseResult()
            {
                var matched = false;
                
                var result = Shience.New<bool>("ComparerFuncReturnsCorrectFalseResult")
                    .Test(control: () => true, candidate: () => false)
                    .WithComparer((a, b) => a == b)
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.False(matched);
            }

            [Fact]
            public void TestThrowsIfCandidateIsNull()
            {
                var science = Shience.New<bool>("NotTestsAreRunIfCandidateIsNull");

                Assert.Throws<ArgumentNullException>(() => science.Test(() => true, null));
            }

            [Fact]
            public void ArgumentNullIsThrownIfControlIsNull()
            {
                var science = Shience.New<bool>("ArgumentNullIsThrownIfControlIsNull");

                Assert.Throws<ArgumentNullException>(() => science.Test(null, () => true));
            }

            [Fact]
            public void TestIsSkippedWhenWhereIsFalse()
            {
                var ran = false;

                var result = Shience.New<bool>("TestIsSkippedWhenWhereIsFalse")
                    .Test(() => true, () => true)
                    .Where(() => false)
                    .PublishTo(_ => ran = true)
                    .Execute();

                Assert.True(result);
                Assert.False(ran);
            }

            [Fact]
            public void TestIsNotSkippedWhenWhereIsTrue()
            {
                var ran = false;

                var result = Shience.New<bool>("TestIsNotSkippedWhenWhereIsTrue")
                    .Test(() => true, () => true)
                    .Where(() => true)
                    .PublishTo(_ => ran = true)
                    .Execute();

                Assert.True(result);
                Assert.True(ran);
            }

            [Fact]
            public void TestIsNotSkippedWhenWhereIsTrueWithMultipleChainedWheres()
            {
                var ran = false;

                var result = Shience.New<bool>("TestIsNotSkippedWhenWhereIsTrueWithMultipleChainedWheres")
                    .Test(() => true, () => true)
                    .Where(() => true)
                    .Where(() => true)
                    .Where(() => true)
                    .PublishTo(_ => ran = true)
                    .Execute();

                Assert.True(result);
                Assert.True(ran);
            }

            [Fact]
            public void TestIsSkippedWhenWhereIsFalseWithMultipleChainedWheres()
            {
                var ran = false;
                
                var result = Shience.New<bool>("TestIsSkippedWhenWhereIsFalseWithMultipleChainedWheres")
                    .Test(() => true, () => true)
                    .Where(() => false)
                    .Where(() => false)
                    .Where(() => false)
                    .PublishTo(_ => ran = true)
                    .Execute();

                Assert.True(result);
                Assert.False(ran);
            }

            [Fact]
            public void TestIsSkippedWhenMultipleChainedWheresWithMixOfTrueFalse()
            {
                var ran = false;
                
                var result = Shience.New<bool>("TestIsSkippedWhenMultipleChainedWheresWithMixOfTrueFalse")
                    .Test(() => true, () => true)
                    .Where(() => false)
                    .Where(() => true)
                    .Where(() => false)
                    .PublishTo(_ => ran = true)
                    .Execute();

                Assert.True(result);
                Assert.False(ran);
            }

            [Fact]
            public void TestThrowsInvalidOperationExceptionIfCalledMultipleTimes()
            {
                var science = Shience.New<bool>("TestThrowsInvalidOperationExceptionIfCalledMultipleTimes")
                    .Test(() => true, () => true);

                Assert.Throws<InvalidOperationException>(() => science.Test(() => true, () => true));
            }

            [Fact]
            public void ExecuteThrowsMismatchExceptionIfRaiseOnMismatchCalled()
            {
                var science = Shience.New<bool>("ExecuteThrowsMismatchExceptionIfRaiseOnMismatchCalled")
                    .Test(() => true, () => false)
                    .RaiseOnMismatch();

                Assert.Throws<MismatchException>(() => science.Execute());
            }

            [Fact]
            public void ExecuteThrowsIfTestNotCalledFirst()
            {
                var science = Shience.New<bool>("ExecuteThrowsIfTestNotCalledFirst");

                Assert.Throws<InvalidOperationException>(() => science.Execute());
            }

            [Fact]
            public async Task ExecuteAsyncThrowsIfTestNotCalledFirst()
            {
                var science = Shience.New<bool>("ExecuteAsyncThrowsIfTestNotCalledFirst");

                await Assert.ThrowsAsync<InvalidOperationException>(() => science.ExecuteAsync());
            }
        }
        
        public sealed class TestAsync
        {
            [Fact]
            public async Task TestsAreRunInParallel()
            {
                var science = Shience.New<bool>("TestsAreRunInParallel");

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
                    })
                    .PublishTo(new FakePublisher().Publish)
                    .ExecuteAsync();

                Assert.Equal("Control", output);
            }
        }
    }
}