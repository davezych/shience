using System.Diagnostics;
using Shience.Publish;
using Shience.Result;

namespace Shience.Test
{
    class FakePublisher<TResult> : IPublisher<TResult>
    {
        public void Publish(ExperimentResult<TResult> result)
        {
            Debug.WriteLine(result.TestName + "|" + result.Matched);
        }
    }
}
