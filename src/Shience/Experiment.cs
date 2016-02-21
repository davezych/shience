using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Shience
{
    public sealed class Experiment<TControlResult, TCandidateResult>
    {
        internal string TestName { get; }
        internal Func<TControlResult> Control { get; set; }
        internal Func<TCandidateResult> Candidate { get; set; }

        internal IList<Func<bool>> Predicates { get; set; } = new List<Func<bool>>();

        internal IList<Action<ExperimentResult<TControlResult, TCandidateResult>>> Publishers { get; } = new List<Action<ExperimentResult<TControlResult, TCandidateResult>>>();
        internal IList<Action<ExperimentResult<TControlResult, TCandidateResult>>> SuccessPublishers { get; } = new List<Action<ExperimentResult<TControlResult, TCandidateResult>>>();
        internal IList<Action<ExperimentResult<TControlResult, TCandidateResult>>> FailurePublishers { get; } = new List<Action<ExperimentResult<TControlResult, TCandidateResult>>>();
        
        internal IList<Func<TestResult<TControlResult>, TestResult<TCandidateResult>, bool>> Comparers { get; set; } = new List<Func<TestResult<TControlResult>, TestResult<TCandidateResult>, bool>>();
        internal IList<Func<TControlResult, TCandidateResult, bool>> ResultComparers { get; set; } = new List<Func<TControlResult, TCandidateResult, bool>>();
        internal IList<Func<Exception, Exception, bool>> ExceptionComparers { get; set; } = new List<Func<Exception, Exception, bool>>();
        internal IList<Func<TimeSpan, TimeSpan, bool>> ExcecutionTimeComparers { get; set; } = new List<Func<TimeSpan, TimeSpan, bool>>();
        
        internal dynamic Context { get; set; }
        internal bool RaiseOnMismatch { get; set; }

        internal Experiment([NotNull]string testName)
        {
            if (string.IsNullOrWhiteSpace(testName))
            {
                throw new ArgumentNullException(nameof(testName));
            }
            TestName = testName;
        }
    }
}
