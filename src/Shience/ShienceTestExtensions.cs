using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Shience.Result;

namespace Shience
{
    public static class ShienceTestExtensions
    {
        public static TResult Test<TResult>(this Science science, Func<TResult> control, Func<TResult> candidate)
        {
            return Test(science, control, candidate, null, null);
        }

        public static TResult Test<TResult>(this Science science, Func<TResult> control, Func<TResult> candidate, params object[] contexts)
        {
            return Test(science, control, candidate, null, contexts);
        }

        public static TResult Test<TResult>(this Science science, Func<TResult> control, Func<TResult> candidate, Func<TResult, TResult, bool> comparer, 
            params object[] contexts)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            //If candidate is null, don't do any science
            if (candidate == null)
            {
                return RunAsync(control).Result;
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = science.TestName,
                ComparerFunc = comparer,
            };

            if (contexts != null)
            {
                experimentResult.Contexts.AddRange(contexts);
            }

            TestResult<TResult> controlResult, candidateResult;

            if (new Random().Next() % 2 == 0)
            {
                experimentResult.ControlRanFirst = true;
                controlResult = InternalTestAsync(control).Result;
                candidateResult = InternalTestAsync(candidate).Result;
            }
            else
            {
                candidateResult = InternalTestAsync(candidate).Result;
                controlResult = InternalTestAsync(control).Result;
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
        
        public static async Task<TResult> TestAsync<TResult>(this Science science, Func<TResult> control, Func<TResult> candidate)
        {
            return await TestAsync(science, control, candidate, null, null);
        }

        public static async Task<TResult> TestAsync<TResult>(this Science science, Func<TResult> control, Func<TResult> candidate, params object[] contexts)
        {
            return await TestAsync(science, control, candidate, null, contexts);
        }

        public static async Task<TResult> TestAsync<TResult>(this Science science, Func<TResult> control, Func<TResult> candidate, Func<TResult, TResult, bool> comparer, params object[] contexts)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            //If candidate is null, don't do any science
            if (candidate == null)
            {
                return await RunAsync(control);
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = science.TestName,
                ComparerFunc = comparer,
            };

            if (contexts != null)
            {
                experimentResult.Contexts.AddRange(contexts);
            }

            var controlTask = InternalTestAsync(control);
            var candidateTask = InternalTestAsync(candidate);

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