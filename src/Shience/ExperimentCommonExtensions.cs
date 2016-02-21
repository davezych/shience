using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Shience
{
    internal static class ExperimentCommonExtensions
    {
        internal static void PublishResults<TControlResult, TCandidateResult>(
            [NotNull]this Experiment<TControlResult, TCandidateResult> experiment,
            [NotNull]ExperimentResult<TControlResult, TCandidateResult> experimentResult)
        {
            experiment.Publishers.ForEach(p => p(experimentResult));

            if (experimentResult.Matched)
            {
                experiment.SuccessPublishers.ForEach(p => p(experimentResult));
                return;
            }

            experiment.FailurePublishers.ForEach(p => p(experimentResult));
        }

        internal static bool AreResultsMatching<TControlResult, TCandidateResult>(
            [NotNull]this Experiment<TControlResult, TCandidateResult> experiment,
            [NotNull]ExperimentResult<TControlResult, TCandidateResult> experimentResult)
        {
            var candidate = experimentResult.CandidateResult;
            var control = experimentResult.ControlResult;
            
            if (!AreComparersGiven(experiment))
                return Equals(candidate, control);
            
            var comparersResult = ApplyComparers(experiment.Comparers, control, candidate);
            var resultComparersResult = ApplyComparers(experiment.ResultComparers, control.Result, candidate.Result);
            var exceptionComparersResult = ApplyComparers(experiment.ExceptionComparers, control.Exception, candidate.Exception);
            var executionTimeComparersResult = ApplyComparers(experiment.ExcecutionTimeComparers, control.RunTime, candidate.RunTime);

            var result = comparersResult 
                && resultComparersResult 
                && exceptionComparersResult 
                && executionTimeComparersResult;

            return result;
        }

        private static bool ApplyComparers<TControlResult, TCandidateResult>(
            [NotNull]IList<Func<TControlResult, TCandidateResult, bool>> comparers,
            TControlResult control, TCandidateResult candidate)
        {
            return !comparers.Any() || comparers.All(compare => compare(control, candidate));
        }

        private static bool AreComparersGiven<TControlResult, TCandidateResult>(
            [NotNull]Experiment<TControlResult, TCandidateResult> experiment)
        {
            return experiment.Comparers.Any() 
                   || experiment.ResultComparers.Any() 
                   || experiment.ExceptionComparers.Any() 
                   || experiment.ExcecutionTimeComparers.Any();
        }
    }
}