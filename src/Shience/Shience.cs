using System;
using JetBrains.Annotations;
using Shience.Publish;

namespace Shience
{
    public static class Shience
    {
        internal static IPublisher Publisher { get; set; }

        public static void SetPublisher([NotNull]IPublisher publisher)
        {
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            Publisher = publisher;
        }
        
        public static Science<TResult> New<TResult>([NotNull]string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            
            if (Publisher == null)
            {
                throw new InvalidOperationException("Call Shience.SetPublisher([NotNull]IPublisher publisher) first.");
            }

            return new Science<TResult>(name, Publisher.Publish);
        }

        public static Science<TResult> New<TResult>([NotNull]string name, [NotNull]IPublisher publisher)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            return new Science<TResult>(name, publisher.Publish);
        }

        public static Science<TResult> New<TResult>([NotNull]string name, [NotNull]Action<ExperimentResult<TResult>> publish)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (publish == null)
            {
                throw new ArgumentNullException(nameof(publish));
            }

            return new Science<TResult>(name, publish);
        }
    }
}
