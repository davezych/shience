using System.Globalization;
using System.Linq;

namespace Shience
{
    internal static class ExperimentCommonExtensions
    {
        internal static void PublishResults<TResult>(this Experiment<TResult> experiment,
            ExperimentResult<TResult> experimentResult)
        {
            experiment.Publishers.ForEach(p => p(experimentResult));

            if (experimentResult.Matched)
            {
                experiment.SuccessPublishers.ForEach(p => p(experimentResult));
                return;
            }

            experiment.FailurePublishers.ForEach(p => p(experimentResult));
        }

        internal static bool AreResultsMatching<TResult>(this Experiment<TResult> experiment,
            ExperimentResult<TResult> experimentResult)
        {
            var candidate = experimentResult.CandidateResult;
            var control = experimentResult.ControlResult;

            return experiment.Comparers.All(compare => compare(control, candidate))
                && experiment.ResultComparers.All(compare => compare(control.Result, candidate.Result)) 
                && experiment.ExceptionComparers.All(compare => compare(control.Exception, candidate.Exception))
                && experiment.ExcecutionTimeComparers.All(compare => compare(control.RunTime, candidate.RunTime));
        }
    }
}