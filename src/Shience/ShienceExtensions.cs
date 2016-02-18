using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shience
{
    public static class ShienceExtensions
    {
        public static Experiment<TResult> PublishTo<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Action<ExperimentResult<TResult>> publish)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (publish == null) throw new ArgumentNullException(nameof(publish));

            experiment.Publish.Add(publish);
            return experiment;
        }

        public static Experiment<TResult> Test<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Func<TResult> control, [NotNull]Func<TResult> candidate)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (candidate == null) throw new ArgumentNullException(nameof(candidate));

            if (experiment.Control != null || experiment.Candidate != null)
            {
                var message = $"{nameof(Test)} may not be called multiple times.";
                throw new InvalidOperationException(message);
            }

            experiment.Control = control;
            experiment.Candidate = candidate;

            return experiment;
        }

        public static Experiment<TResult> WithComparer<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Func<TResult, TResult, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            if (experiment.Comparer != null)
            {
                var message = $"{nameof(WithComparer)} may not be called multiple times.";
                throw new InvalidOperationException(message);
            }

            experiment.Comparer = comparer;

            return experiment;
        }

        public static Experiment<TResult> WithContext<TResult>(
            [NotNull]this Experiment<TResult> experiment, dynamic context)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            
            experiment.Context = context; // may be null
            return experiment;
        }

        public static Experiment<TResult> Where<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Func<bool> predicate)
        {
            if(experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            experiment.Predicates.Add(predicate);

            return experiment;
        }

        public static Experiment<TResult> RaiseOnMismatch<TResult>([NotNull]this Experiment<TResult> experiment)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));

            experiment.RaiseOnMismatch = true;

            return experiment;
        }

        public static TResult Execute<TResult>([NotNull]this Experiment<TResult> experiment)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));

            if (experiment.Control == null || experiment.Candidate == null)
            {
                throw new InvalidOperationException(
                    "Call Experiment.Test<TResult>(this Experiment<TResult> Experiment, Func<TResult> control, Func<TResult> candidate) first.");
            }

            if (experiment.Predicates.Any(p => !p()))
            {
                return RunAsync(experiment.Control).Result;
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = experiment.TestName,
                ComparerFunc = experiment.Comparer,
                Context = experiment.Context,
                UtcStartDate = DateTime.UtcNow,
            };

            TestResult<TResult> controlResult, candidateResult;

            if (new Random().Next() % 2 == 0)
            {
                experimentResult.ControlRanFirst = true;
                controlResult = InternalTestAsync(experiment.Control).Result;
                candidateResult = InternalTestAsync(experiment.Candidate).Result;
            }
            else
            {
                candidateResult = InternalTestAsync(experiment.Candidate).Result;
                controlResult = InternalTestAsync(experiment.Control).Result;
            }

            experimentResult.ControlResult = controlResult;
            experimentResult.CandidateResult = candidateResult;

            experiment.Publish.ForEach(p => p(experimentResult));
            
            if (controlResult.Exception != null)
            {
                throw controlResult.Exception;
            }

            if (experiment.RaiseOnMismatch)
            {
                throw new MismatchException($"Control: {controlResult.Result}, Candidate: {candidateResult.Result}");
            }

            return controlResult.Result;
        }

        public static async Task<TResult> ExecuteAsync<TResult>([NotNull]this Experiment<TResult> experiment)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));

            if (experiment.Control == null || experiment.Candidate == null)
            {
                throw new InvalidOperationException(
                    "Call Experiment.Test<TResult>(this Experiment<TResult> Experiment, Func<TResult> control, Func<TResult> candidate) first.");
            }

            if (experiment.Predicates.Any(p => !p()))
            {
                return RunAsync(experiment.Control).Result;
            }

            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = experiment.TestName,
                ComparerFunc = experiment.Comparer,
                Context = experiment.Context,
                UtcStartDate = DateTime.UtcNow,
            };

            var controlTask = InternalTestAsync(experiment.Control);
            var candidateTask = InternalTestAsync(experiment.Candidate);

            experimentResult.ControlResult = await controlTask;
            experimentResult.CandidateResult = await candidateTask;

            experiment.Publish.ForEach(p => p(experimentResult));

            if (experimentResult.ControlResult.Exception != null)
            {
                throw experimentResult.ControlResult.Exception;
            }

            if (experiment.RaiseOnMismatch)
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