using System;
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
        internal dynamic Contexts { get; set; }
        internal bool Skip { get; set; }

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
