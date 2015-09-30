using System;
using System.Diagnostics;
using Shience.Publish;
using Shience.Result;

namespace Shience
{
    public class Science<TResult>
    {
        private readonly string _testName;
        private readonly IPublisher _publisher;

        internal Science(string testName, IPublisher publisher)
        {
            if (string.IsNullOrWhiteSpace(testName))
            {
                throw new ArgumentNullException(nameof(testName));
            }
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            _testName = testName;
            _publisher = publisher;
        }

        public TResult Test(Func<TResult> control, Func<TResult> candidate)
        {
            return Test(control, candidate, null, null);
        }

        public TResult Test(Func<TResult> control, Func<TResult> candidate, params object[] contexts)
        {
            return Test(control, candidate, null, contexts);
        }

        public TResult Test(Func<TResult> control, Func<TResult> candidate, Func<TResult, TResult, bool> comparer, params object[] contexts)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            //If candidate is null, don't do any science
            if (candidate == null)
            {
                return Run(control);
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = _testName,
                ComparerFunc = comparer,
            };

            if (contexts != null)
            {
                experimentResult.Contexts.AddRange(contexts);
            }

            TestResult<TResult> controlResult, candidateResult;

            if (new Random().Next()%2 == 0)
            {
                experimentResult.ControlRanFirst = true;
                controlResult = InternalTest(control);
                candidateResult = InternalTest(candidate);
            }
            else
            {
                candidateResult = InternalTest(candidate);
                controlResult = InternalTest(control);
            }

            experimentResult.ControlResult = controlResult;
            experimentResult.CandidateResult = candidateResult;

            _publisher.Publish(experimentResult);

            if (controlResult.Exception != null)
            {
                throw controlResult.Exception;
            }

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
