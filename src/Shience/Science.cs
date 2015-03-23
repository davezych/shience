using System;
using System.Diagnostics;

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
            var controlResult = InternalTest(control);
            var candidateResult = InternalTest(candidate);

            return controlResult.Result;
        }

        private TestResult<TResult> InternalTest(Func<TResult> action)
        {
            var tr = new TestResult<TResult>();
            var sw = new Stopwatch();

            sw.Start();

            try
            {
                var result = Run(action);
                tr.Result = result;
            }
            catch (Exception e)
            {
                tr.Exception = e;
            }

            sw.Stop();

            tr.RunTime = sw.ElapsedMilliseconds;
            return tr;
        }

        private TResult Run(Func<TResult> action)
        {
            return action();
        }
    }
}
