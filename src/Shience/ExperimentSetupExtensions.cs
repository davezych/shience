using JetBrains.Annotations;
using System;

namespace Shience
{
    public static class ExperimentSetupExtensions
    {
        public static Experiment<TControl, TCandidate> Test<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Func<TControl> control, [NotNull]Func<TCandidate> candidate)
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

        public static Experiment<TControl, TCandidate> PublishTo<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Action<ExperimentResult<TControl, TCandidate>> publish)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (publish == null) throw new ArgumentNullException(nameof(publish));

            experiment.Publishers.Add(publish);
            return experiment;
        }

        public static Experiment<TControl, TCandidate> PublishSuccessTo<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Action<ExperimentResult<TControl, TCandidate>> publish)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (publish == null) throw new ArgumentNullException(nameof(publish));

            experiment.SuccessPublishers.Add(publish);
            return experiment;
        }

        public static Experiment<TControl, TCandidate> PublishFailureTo<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Action<ExperimentResult<TControl, TCandidate>> publish)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (publish == null) throw new ArgumentNullException(nameof(publish));

            experiment.FailurePublishers.Add(publish);
            return experiment;
        }

        public static Experiment<TControl, TCandidate> WithComparer<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Func<TestResult<TControl>, TestResult<TCandidate>, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            experiment.Comparers.Add(comparer);

            return experiment;
        }

        public static Experiment<TControl, TCandidate> WithResultComparer<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Func<TControl, TCandidate, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            experiment.ResultComparers.Add(comparer);

            return experiment;
        }

        public static Experiment<TControl, TCandidate> WithExceptionComparer<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Func<Exception, Exception, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            experiment.ExceptionComparers.Add(comparer);

            return experiment;
        }

        public static Experiment<TControl, TCandidate> WithExecutionTimeComparer<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Func<TimeSpan, TimeSpan, bool> comparer)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            
            experiment.ExcecutionTimeComparers.Add(comparer);

            return experiment;
        }

        public static Experiment<TControl, TCandidate> WithContext<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, dynamic context)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));
            
            experiment.Context = context; // may be null
            return experiment;
        }

        public static Experiment<TControl, TCandidate> Where<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment, 
            [NotNull]Func<bool> predicate)
        {
            if(experiment == null) throw new ArgumentNullException(nameof(experiment));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            experiment.Predicates.Add(predicate);

            return experiment;
        }

        public static Experiment<TControl, TCandidate> RaiseOnMismatch<TControl, TCandidate>(
            [NotNull]this Experiment<TControl, TCandidate> experiment)
        {
            if (experiment == null) throw new ArgumentNullException(nameof(experiment));

            experiment.RaiseOnMismatch = true;

            return experiment;
        }
    }
}