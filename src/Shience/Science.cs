using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Shience
{
    public sealed class Science<TResult>
    {
        internal string TestName { get; }
        internal IList<Action<ExperimentResult<TResult>>> Publish { get; } = new List<Action<ExperimentResult<TResult>>>();
        internal Func<TResult> Control { get; set; }
        internal Func<TResult> Candidate { get; set; }
        internal Func<TResult, TResult, bool> Comparer { get; set; }
        internal dynamic Context { get; set; }
        internal IList<Func<bool>> Predicates { get; set; } = new List<Func<bool>>();
        internal bool RaiseOnMismatch { get; set; }

        internal Science([NotNull]string testName)
        {
            if (string.IsNullOrWhiteSpace(testName))
            {
                throw new ArgumentNullException(nameof(testName));
            }
            TestName = testName;
        }
    }
}
