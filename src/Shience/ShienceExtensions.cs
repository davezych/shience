using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shience
{
    public static class ShienceExtensions
    {
        public static Science<TResult> PublishTo<TResult>(
            [NotNull]this Science<TResult> science, [NotNull]Action<ExperimentResult<TResult>> publish)
        {
            if (science == null) throw new ArgumentNullException(nameof(science));
            if (publish == null) throw new ArgumentNullException(nameof(publish));

            science.Publish.Add(publish);
            return science;
        }

        public static Science<TResult> Test<TResult>(
            [NotNull]this Science<TResult> science, [NotNull]Func<TResult> control, [NotNull]Func<TResult> candidate)
        {
            if (science == null) throw new ArgumentNullException(nameof(science));
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (candidate == null) throw new ArgumentNullException(nameof(candidate));

            if (science.Control != null || science.Candidate != null)
            {
                var message = $"{nameof(Test)} may not be called multiple times.";
                throw new InvalidOperationException(message);
            }

            science.Control = control;
            science.Candidate = candidate;

            return science;
        }

        public static Science<TResult> WithComparer<TResult>(
            [NotNull]this Science<TResult> science, [NotNull]Func<TResult, TResult, bool> comparer)
        {
            if (science == null) throw new ArgumentNullException(nameof(science));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            if (science.Comparer != null)
            {
                var message = $"{nameof(WithComparer)} may not be called multiple times.";
                throw new InvalidOperationException(message);
            }

            science.Comparer = comparer;

            return science;
        }

        public static Science<TResult> WithContext<TResult>(
            [NotNull]this Science<TResult> science, dynamic context)
        {
            if (science == null) throw new ArgumentNullException(nameof(science));
            
            science.Context = context; // may be null
            return science;
        }

        public static Science<TResult> Where<TResult>(
            [NotNull]this Science<TResult> science, [NotNull]Func<bool> predicate)
        {
            if(science == null) throw new ArgumentNullException(nameof(science));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            science.Predicates.Add(predicate);

            return science;
        }

        public static Science<TResult> RaiseOnMismatch<TResult>([NotNull]this Science<TResult> science)
        {
            if (science == null) throw new ArgumentNullException(nameof(science));

            science.RaiseOnMismatch = true;

            return science;
        }

        public static TResult Execute<TResult>([NotNull]this Science<TResult> science)
        {
            if (science == null) throw new ArgumentNullException(nameof(science));

            if (science.Control == null || science.Candidate == null)
            {
                throw new InvalidOperationException(
                    "Call Science.Test<TResult>(this Science<TResult> science, Func<TResult> control, Func<TResult> candidate) first.");
            }

            if (science.Predicates.Any(p => !p()))
            {
                return RunAsync(science.Control).Result;
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = science.TestName,
                ComparerFunc = science.Comparer,
            };

            experimentResult.Context = science.Context;
            
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

            science.Publish.ForEach(p => p(experimentResult));
            
            if (controlResult.Exception != null)
            {
                throw controlResult.Exception;
            }

            if (science.RaiseOnMismatch)
            {
                throw new MismatchException($"Control: {controlResult.Result}, Candidate: {candidateResult.Result}");
            }

            return controlResult.Result;
        }

        public static async Task<TResult> ExecuteAsync<TResult>([NotNull]this Science<TResult> science)
        {
            if (science == null) throw new ArgumentNullException(nameof(science));

            if (science.Control == null || science.Candidate == null)
            {
                throw new InvalidOperationException(
                    "Call Science.Test<TResult>(this Science<TResult> science, Func<TResult> control, Func<TResult> candidate) first.");
            }

            if (science.Predicates.Any(p => !p()))
            {
                return RunAsync(science.Control).Result;
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = science.TestName,
                ComparerFunc = science.Comparer,
            };

            experimentResult.Context = science.Context;
            
            var controlTask = InternalTestAsync(science.Control);
            var candidateTask = InternalTestAsync(science.Candidate);

            experimentResult.ControlResult = await controlTask;
            experimentResult.CandidateResult = await candidateTask;

            science.Publish.ForEach(p => p(experimentResult));

            if (experimentResult.ControlResult.Exception != null)
            {
                throw experimentResult.ControlResult.Exception;
            }

            if (science.RaiseOnMismatch)
            {
                throw new MismatchException($"Control: {experimentResult.ControlResult.Result}, Candidate: {experimentResult.CandidateResult.Result}");
            }

            return experimentResult.ControlResult.Result;
        }

        private static async Task<TestResult<TResult>> InternalTestAsync<TResult>([NotNull]Func<TResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var testResult = new TestResult<TResult>();
            var timer = new Stopwatch();

            timer.Start();
            try
            {
                var result = RunAsync(action);
                testResult.Result = await result;
            }
            catch (Exception e)
            {
                testResult.Exception = e;
            }
            finally
            {
                timer.Stop();
            }
            
            testResult.RunTime = timer.ElapsedMilliseconds;
            return testResult;
        }

        private static async Task<TResult> RunAsync<TResult>([NotNull]Func<TResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var result = await Task.Run(action);
            return result;
        }
    }
}