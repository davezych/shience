using System;
using JetBrains.Annotations;
using Shience.Publish;

namespace Shience
{
    public sealed class Science
    {
        internal string TestName { get; }
        internal IPublisher Publisher { get; }

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
