using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Shience
{
    public sealed class Science<TResult>
    {
        internal string TestName { get; }
        internal Action<ExperimentResult<TResult>> Publish { get; }
        internal Func<TResult> Control { get; set; }
        internal Func<TResult> Candidate { get; set; }
        internal Func<TResult, TResult, bool> Comparer { get; set; }
        internal IList<object> Contexts { get; } = new List<object>();
        internal IList<Func<bool>> Predicates { get; set; } = new List<Func<bool>>();
        internal bool RaiseOnMismatch { get; set; }

        internal Science([NotNull]string testName, [NotNull]Action<ExperimentResult<TResult>> publish)
        {
            if (string.IsNullOrWhiteSpace(testName))
            {
                throw new ArgumentNullException(nameof(testName));
            }
            if (publish == null)
            {
                throw new ArgumentNullException(nameof(publish));
            }

            TestName = testName;
            Publish = publish;
        }
    }
}
