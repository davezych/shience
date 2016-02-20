using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Shience
{
    public sealed class Experiment<TResult>
    {
        internal string TestName { get; }
        internal Func<TResult> Control { get; set; }
        internal Func<TResult> Candidate { get; set; }

        internal IList<Func<bool>> Predicates { get; set; } = new List<Func<bool>>();

        internal IList<Action<ExperimentResult<TResult>>> Publishers { get; } = new List<Action<ExperimentResult<TResult>>>();
        internal IList<Action<ExperimentResult<TResult>>> SuccessPublishers { get; } = new List<Action<ExperimentResult<TResult>>>();
        internal IList<Action<ExperimentResult<TResult>>> FailurePublishers { get; } = new List<Action<ExperimentResult<TResult>>>();
        
        internal IList<Func<TestResult<TResult>, TestResult<TResult>, bool>> Comparers { get; set; } = new List<Func<TestResult<TResult>, TestResult<TResult>, bool>>();
        internal IList<Func<TResult, TResult, bool>> ResultComparers { get; set; } = new List<Func<TResult, TResult, bool>>();
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
