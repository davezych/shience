using System;
using System.Threading;
using System.Threading.Tasks;
using Shience.Test.Fakes;
using Shience.Test.TestObjects;
using Xunit;

namespace Shience.Test
{
    public sealed class ExperimentTests
    {
        public sealed class Test
        {
            [Fact]
            public void DefaultComparerReturnsTrueWithSameResultOnPrimitives()
            {
                var matched = false;
                var testName = "DefaultComparerReturnsTrueWithSameResultOnPrimitives";

                var result = Science.New<bool>(testName)
                    .Test(control: () => true, candidate: () => true)
                    .PublishTo(r => matched = r.Matched)
                    .Execute();
                
                Assert.True(matched);
            }

            [Fact]
            public void DefaultComparerReturnsFalseWithDifferentResultOnPrimitives()
            {
                var matched = false;
                var result = Science.New<bool>("DefaultComparerReturnsFalseWithDifferentResultOnPrimitives")
                    .Test(control: () => true, candidate: () => false)
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.False(matched);
            }

            [Fact]
            public void DefaultComparerReturnsTrueWithSameResultOnObject()
            {
                var matched = false;
                var result = Science.New<TestNumber>("DefaultComparerReturnsTrueWithSameResultOnObject")
                    .Test(control: () => new TestNumber(1), candidate: () => new TestNumber(1))
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.True(matched);
            }

            [Fact]
            public void DefaultComparerReturnsFalseWithDifferentResultOnObject()
            {
                var matched = false;
                var result = Science.New<TestNumber>("DefaultComparerReturnsFalseWithDifferentResultOnObject")
                    .Test(control: () => new TestNumber(1), candidate: () => new TestNumber(2))
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.False(matched);
            }

            [Fact]
            public void ComparerFuncReturnsCorrectTrueResult()
            {
                var matched = false;
                var result = Science.New<bool>("ComparerFuncReturnsCorrectTrueResult")
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
                
                var result = Science.New<bool>("ComparerFuncReturnsCorrectFalseResult")
                    .Test(control: () => true, candidate: () => false)
                    .WithComparer((a, b) => a == b)
                    .PublishTo(e => matched = e.Matched)
                    .Execute();

                Assert.False(matched);
            }

            [Fact]
            public void TestThrowsIfCandidateIsNull()
            {
                var science = Science.New<bool>("NotTestsAreRunIfCandidateIsNull");

                Assert.Throws<ArgumentNullException>(() => science.Test(() => true, null));
            }

            [Fact]
            public void ArgumentNullIsThrownIfControlIsNull()
            {
                var science = Science.New<bool>("ArgumentNullIsThrownIfControlIsNull");

                Assert.Throws<ArgumentNullException>(() => science.Test(null, () => true));
            }

            [Fact]
            public void TestIsSkippedWhenWhereIsFalse()
            {
                var ran = false;

                var result = Science.New<bool>("TestIsSkippedWhenWhereIsFalse")
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

                var result = Science.New<bool>("TestIsNotSkippedWhenWhereIsTrue")
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

                var result = Science.New<bool>("TestIsNotSkippedWhenWhereIsTrueWithMultipleChainedWheres")
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
                
                var result = Science.New<bool>("TestIsSkippedWhenWhereIsFalseWithMultipleChainedWheres")
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
                
                var result = Science.New<bool>("TestIsSkippedWhenMultipleChainedWheresWithMixOfTrueFalse")
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
                var science = Science.New<bool>("TestThrowsInvalidOperationExceptionIfCalledMultipleTimes")
                    .Test(() => true, () => true);

                Assert.Throws<InvalidOperationException>(() => science.Test(() => true, () => true));
            }

            [Fact]
            public void ExecuteThrowsMismatchExceptionIfRaiseOnMismatchCalled()
            {
                var science = Science.New<bool>("ExecuteThrowsMismatchExceptionIfRaiseOnMismatchCalled")
                    .Test(() => true, () => false)
                    .RaiseOnMismatch();

                Assert.Throws<MismatchException>(() => science.Execute());
            }

            [Fact]
            public void ExecuteThrowsIfTestNotCalledFirst()
            {
                var science = Science.New<bool>("ExecuteThrowsIfTestNotCalledFirst");

                Assert.Throws<InvalidOperationException>(() => science.Execute());
            }

            [Fact]
            public async Task ExecuteAsyncThrowsIfTestNotCalledFirst()
            {
                var science = Science.New<bool>("ExecuteAsyncThrowsIfTestNotCalledFirst");

                await Assert.ThrowsAsync<InvalidOperationException>(() => science.ExecuteAsync());
            }

            [Fact]
            public void ExecuteRecordsStartTimeInUtcOnExperimentResult()
            {
                var experiment = Science.New<bool>("ExecuteRecordsStartTimeInUtcOnExperimentResult");

                var startDate = DateTime.Now;

                experiment.Test(() => true, () => true)
                    .PublishTo((e) => startDate = e.UtcStartDate)
                    .Execute();

                Assert.True(startDate != DateTime.MinValue);
                Assert.True(startDate.Kind == DateTimeKind.Utc);
            }

            [Fact]
            public async Task ExecuteAsyncRecordsStartTimeInUtcOnExperimentResult()
            {
                var experiment = Science.New<bool>("ExecuteRecordsStartTimeInUtcOnExperimentResult");

                var startDate = DateTime.Now;

                await experiment.Test(() => true, () => true)
                    .PublishTo((e) => startDate = e.UtcStartDate)
                    .ExecuteAsync();

                Assert.True(startDate != DateTime.MinValue);
                Assert.True(startDate.Kind == DateTimeKind.Utc);
            }
        }
        
        public sealed class TestAsync
        {
            [Fact]
            public async Task TestsAreRunInParallel()
            {
                var science = Science.New<bool>("TestsAreRunInParallel");

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