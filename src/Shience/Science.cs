using System;

namespace Shience
{
    public class Science<TResult>
    {
        private readonly string _testName;

        internal Science(string testName)
        {
            _testName = testName;
        }

        public TResult Test(Func<TResult> control, Func<TResult> candidate)
        {
            var controlResult = control();
            var candidateResult = candidate();

            return controlResult;
        }
    }
}
