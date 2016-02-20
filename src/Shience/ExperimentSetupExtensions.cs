using JetBrains.Annotations;
using System;

namespace Shience
{
    public static class ExperimentSetupExtensions
    {
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

        public static Experiment<TResult> PublishTo<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Action<ExperimentResult<TResult>> publish)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (publish == null) throw new ArgumentNullException(nameof(publish));

            experiment.Publishers.Add(publish);
            return experiment;
        }

        public static Experiment<TResult> PublishSuccessTo<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Action<ExperimentResult<TResult>> publish)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (publish == null) throw new ArgumentNullException(nameof(publish));

            experiment.SuccessPublishers.Add(publish);
            return experiment;
        }

        public static Experiment<TResult> PublishFailureTo<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Action<ExperimentResult<TResult>> publish)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (publish == null) throw new ArgumentNullException(nameof(publish));

            experiment.FailurePublishers.Add(publish);
            return experiment;
        }

        public static Experiment<TResult> WithComparer<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Func<TestResult<TResult>, TestResult<TResult>, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            experiment.Comparers.Add(comparer);

            return experiment;
        }

        public static Experiment<TResult> WithResultComparer<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Func<TResult, TResult, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            experiment.ResultComparers.Add(comparer);

            return experiment;
        }

        public static Experiment<TResult> WithExceptionComparer<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Func<Exception, Exception, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            experiment.ExceptionComparers.Add(comparer);

            return experiment;
        }

        public static Experiment<TResult> WithExecutionTimeComparer<TResult>(
            [NotNull]this Experiment<TResult> experiment, [NotNull]Func<TimeSpan, TimeSpan, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            experiment.ExcecutionTimeComparers.Add(comparer);

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
    }
}