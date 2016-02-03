using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Shience.Publish;
using Shience.Result;

namespace Shience
{
    public class Science<TResult>
    {
        private readonly string _testName;
        private readonly IPublisher _publisher;

        private Func<TResult> _control;
        private Func<TResult> _candidate;
        private Func<TResult, TResult, bool> _comparer;
        private object[] _contexts;

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

        public Science<TResult> Test(Func<TResult> control, Func<TResult> candidate)
        {
            _control = control;
            _candidate = candidate;

            return this;
        }

        public Science<TResult> WithComparer(Func<TResult, TResult, bool> comparer)
        {
            _comparer = comparer;

            return this;
        }

        public Science<TResult> WithContext(params object[] contexts)
        {
            _contexts = contexts;

            return this;
        }

        public TResult Execute()
        {
            if (_control == null)
            {
                throw new ArgumentNullException(nameof(_control));
            }

            //If candidate is null, don't do any science
            if (_candidate == null)
            {
                return RunAsync(_control).Result;
            }
         
            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = _testName,
                ComparerFunc = _comparer,
            };

            if (_contexts != null)
            {
                experimentResult.Contexts.AddRange(_contexts);
            }

            TestResult<TResult> controlResult, candidateResult;

            if (new Random().Next() % 2 == 0)
            {
                experimentResult.ControlRanFirst = true;
                controlResult = InternalTestAsync(_control).Result;
                candidateResult = InternalTestAsync(_candidate).Result;
            }
            else
            {
                candidateResult = InternalTestAsync(_candidate).Result;
                controlResult = InternalTestAsync(_control).Result;
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

        public async Task<TResult> ExecuteAsync()
        {
            if (_control == null)
            {
                throw new ArgumentNullException(nameof(_control));
            }

            //If candidate is null, don't do any science
            if (_candidate == null)
            {
                return RunAsync(_control).Result;
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = _testName,
                ComparerFunc = _comparer,
            };

            if (_contexts != null)
            {
                experimentResult.Contexts.AddRange(_contexts);
            }

            var controlTask = InternalTestAsync(_control);
            var candidateTask = InternalTestAsync(_candidate);

            experimentResult.ControlResult = await controlTask;
            experimentResult.CandidateResult = await candidateTask;

            _publisher.Publish(experimentResult);

            if (experimentResult.ControlResult.Exception != null)
            {
                throw experimentResult.ControlResult.Exception;
            }

            return experimentResult.ControlResult.Result;
        }
        
        private async Task<TestResult<TResult>> InternalTestAsync(Func<TResult> action)
        {
            var tr = new TestResult<TResult>();
            var sw = new Stopwatch();

            sw.Start();

            try
            {
                var result = RunAsync(action);
                tr.Result = await result;
            }
            catch (Exception e)
            {
                tr.Exception = e;
            }

            sw.Stop();

            tr.RunTime = sw.ElapsedMilliseconds;
            return tr;
        }

        private async Task<TResult> RunAsync(Func<TResult> action)
        {
            var result = await Task.Run<TResult>(action);

            return result;
        }
    }
}
