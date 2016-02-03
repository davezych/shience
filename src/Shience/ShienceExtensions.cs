using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Shience.Result;

namespace Shience
{
    public static class ShienceExtensions
    {
        public static Science<TResult> Test<TResult>(this Science<TResult> science, Func<TResult> control, Func<TResult> candidate)
        {
            science.Control = control;
            science.Candidate = candidate;

            return science;
        }

        public static Science<TResult> WithComparer<TResult>(this Science<TResult> science, Func<TResult, TResult, bool> comparer)
        {
            science.Comparer = comparer;

            return science;
        }

        public static Science<TResult> WithContext<TResult>(this Science<TResult> science, params object[] contexts)
        {
            science.Contexts.Clear();
            science.Contexts.Add(contexts);

            return science;
        }

        public static TResult Execute<TResult>(this Science<TResult> science)
        {
            if (science.Control == null)
            {
                throw new ArgumentNullException(nameof(science.Control));
            }

            //If candidate is null, don't do any science
            if (science.Candidate == null)
            {
                return RunAsync(science.Control).Result;
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = science.TestName,
                ComparerFunc = science.Comparer,
            };

            if (science.Contexts != null)
            {
                experimentResult.Contexts.AddRange(science.Contexts);
            }

            TestResult<TResult> controlResult, candidateResult;

            if (new Random().Next() % 2 == 0)
            {
                experimentResult.ControlRanFirst = true;
                controlResult = InternalTestAsync(science.Control).Result;
                candidateResult = InternalTestAsync(science.Candidate).Result;
            }
            else
            {
                candidateResult = InternalTestAsync(science.Candidate).Result;
                controlResult = InternalTestAsync(science.Control).Result;
            }

            experimentResult.ControlResult = controlResult;
            experimentResult.CandidateResult = candidateResult;

            science.Publisher.Publish(experimentResult);

            if (controlResult.Exception != null)
            {
                throw controlResult.Exception;
            }

            return controlResult.Result;
        }

        public static async Task<TResult> ExecuteAsync<TResult>(this Science<TResult> science)
        {
            if (science.Control == null)
            {
                throw new ArgumentNullException(nameof(science.Control));
            }

            //If candidate is null, don't do any science
            if (science.Candidate == null)
            {
                return RunAsync(science.Control).Result;
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = science.TestName,
                ComparerFunc = science.Comparer,
            };

            if (science.Contexts != null)
            {
                experimentResult.Contexts.AddRange(science.Contexts);
            }

            var controlTask = InternalTestAsync(science.Control);
            var candidateTask = InternalTestAsync(science.Candidate);

            experimentResult.ControlResult = await controlTask;
            experimentResult.CandidateResult = await candidateTask;

            science.Publisher.Publish(experimentResult);

            if (experimentResult.ControlResult.Exception != null)
            {
                throw experimentResult.ControlResult.Exception;
            }

            return experimentResult.ControlResult.Result;
        }

        private static async Task<TestResult<TResult>> InternalTestAsync<TResult>(Func<TResult> action)
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

        private static async Task<TResult> RunAsync<TResult>(Func<TResult> action)
        {
            var result = await Task.Run(action);
            return result;
        }
    }
}