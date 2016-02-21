using Xunit;

namespace Shience.Test
{
    public sealed class PublisherTests
    {
        [Fact]
        public void PublishIsCalledAfterExecute()
        {
            var experiment = Science.New<bool>("PublishIsCalledAfterExecute");

            var matched = false;

            experiment.Test<bool, bool>(control: () => true, candidate: () => true)
                .PublishTo((e) => matched = e.Matched)
                .Execute();

            Assert.True(matched);
        }

        [Fact]
        public void AllPublishersAreCalledAfterExecute()
        {
            var experiment = Science.New<bool>("AllPublishersAreCalledAfterExecute");

            var matchedOne = false;
            var matchedTwo = false;
            var matchedThree = false;

            experiment.Test<bool, bool>(control: () => true, candidate: () => true)
                .PublishTo((e) => matchedOne = e.Matched)
                .PublishTo((e) => matchedTwo = e.Matched)
                .PublishTo((e) => matchedThree = e.Matched)
                .Execute();

            Assert.True(matchedOne);
            Assert.True(matchedTwo);
            Assert.True(matchedThree);
        }

        [Fact]
        public void ExecuteRunsWithNoPublisher()
        {
            var experiment = Science.New<bool>("PublishIsCalledAfterExecute");

            var result = experiment.Test<bool, bool>(control: () => true, candidate: () => true)
                .Execute();

            Assert.True(result);
        }
    }
}