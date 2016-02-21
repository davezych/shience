using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Shience
{
    public static class ExperimentExecuteAsyncExtensions
    {
        public static async Task<TResult> ExecuteAsync<TResult>([NotNull]this Experiment<TResult> experiment)
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

            var experimentResult = await ExecuteExperimentAsync(experiment);
            experimentResult.Matched = experiment.AreResultsMatching(experimentResult);
            experiment.PublishResults(experimentResult);

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

        private static async Task<ExperimentResult<TResult>> ExecuteExperimentAsync<TResult>(this Experiment<TResult> experiment)
        {
            var experimentResult = new ExperimentResult<TResult>
            {
                TestName = experiment.TestName,
                Context = experiment.Context
            };

            var controlTask = experiment.Control.InternalTestAsync();
            var candidateTask = experiment.Candidate.InternalTestAsync();

            experimentResult.ControlResult = await controlTask;
            experimentResult.CandidateResult = await candidateTask;
            return experimentResult;
        }
    }
}