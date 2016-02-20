using System;
using System.Linq;
using JetBrains.Annotations;

namespace Shience
{
    public static class ExperimentExecuteExtensions
    {
        public static TResult Execute<TResult>([NotNull] this Experiment<TResult> experiment)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));

            if (experiment.Control == null || experiment.Candidate == null)
            {
                throw new InvalidOperationException(
                    "Call Experiment.Test<TResult>(this Experiment<TResult> experiment, Func<TResult> control, Func<TResult> candidate) first.");
            }

            if (experiment.Predicates.Any(p => !p()))
            {
                return experiment.Control.RunAsync().Result;
            }

            var experimentResult = ExecuteExperiment(experiment);

            experimentResult.Matched = experiment.AreResultsMatching(experimentResult);
            experiment.PublishResults(experimentResult);

            if (experimentResult.ControlResult.Exception != null)
            {
                throw experimentResult.ControlResult.Exception;
            }

            if (experiment.RaiseOnMismatch && !experimentResult.Matched)
            {
                throw new MismatchException(
                    $"Control: {experimentResult.ControlResult.Result}, Candidate: {experimentResult.CandidateResult.Result}");
            }

            return experimentResult.ControlResult.Result;
        }

        private static readonly Lazy<Random> RandomSingleton = new Lazy<Random>(() => new Random());

        private static ExperimentResult<TResult> ExecuteExperiment<TResult>(Experiment<TResult> experiment)
        {
            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = experiment.TestName,
                Context = experiment.Context
            };

            if (RandomSingleton.Value.Next()%2 == 0)
            {
                experimentResult.ControlRanFirst = true;
                experimentResult.ControlResult = experiment.Control.InternalTestAsync().Result;
                experimentResult.CandidateResult = experiment.Candidate.InternalTestAsync().Result;
            }
            else
            {
                experimentResult.ControlRanFirst = false;
                experimentResult.CandidateResult = experiment.Candidate.InternalTestAsync().Result;
                experimentResult.ControlResult = experiment.Control.InternalTestAsync().Result;
            }

            return experimentResult;
        }
    }
}