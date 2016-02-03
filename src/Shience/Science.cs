using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Shience.Publish;

namespace Shience
{
    public sealed class Science<TResult>
    {
        internal string TestName { get; }
        internal IPublisher Publisher { get; }
        internal Func<TResult> Control { get; set; }
        internal Func<TResult> Candidate { get; set; }
        internal Func<TResult, TResult, bool> Comparer { get; set; }
        internal IList<object> Contexts { get; } = new List<object>();

        internal Science([NotNull]string testName, [NotNull]IPublisher publisher)
        {
            if (string.IsNullOrWhiteSpace(testName))
            {
                throw new ArgumentNullException(nameof(testName));
            }
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            TestName = testName;
            Publisher = publisher;
        }


    }
}
