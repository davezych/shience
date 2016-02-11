using System.Collections.Generic;

namespace Shience.Test.Fakes
{
    internal class FakePublisher
    {
        public void Publish<TResult>(ExperimentResult<TResult> result)
        {
            PublishingResults.TestNamesWithResults.Add(result.TestName, result.Matched);
        }
    }

    internal static class PublishingResults
    {
        public static Dictionary<string, bool> TestNamesWithResults;

        static PublishingResults()
        {
            TestNamesWithResults = new Dictionary<string, bool>();
        }
    }
}
