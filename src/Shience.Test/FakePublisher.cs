using System.Collections.Generic;
using Shience.Publish;
using Shience.Result;

namespace Shience.Test
{
    internal class FakePublisher<TResult> : IPublisher<TResult>
    {
        public void Publish(ExperimentResult<TResult> result)
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
