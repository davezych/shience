using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shience
{
    public class Science<TResult>
    {
        private readonly string _testName;
        private readonly IComparer<TResult> _comparer;

        internal Science(string testName)
        {
            _testName = testName;
        }

        internal Science(string testName, IComparer<TResult> comparer)
        {
            _testName = testName;
            _comparer = comparer;
        }

        public TResult Test(Func<TResult> control, Func<TResult> candidate)
        {
            var experimentResult = new ExperimentResult<TResult>(_comparer);
            experimentResult.TestName = _testName;

            var controlResult = InternalTest(control);
            var candidateResult = InternalTest(candidate);

            experimentResult.ControlResult = controlResult;
            experimentResult.CandidateResult = candidateResult;

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
