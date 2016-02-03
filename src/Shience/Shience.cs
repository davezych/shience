using System;
using JetBrains.Annotations;
using Shience.Publish;

namespace Shience
{
    public static class Shience
    {
        private static IPublisher _publisher;

        public static void SetPublisher([NotNull]IPublisher publisher)
        {
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            _publisher = publisher;
        }
        
        public static Science New([NotNull]string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            // TODO: test this 
            if (_publisher == null)
            {
                throw new InvalidOperationException("Call Shience.SetPublisher([NotNull]IPublisher publisher) first.");
            }

            return new Science(name, _publisher);
        }

        public static Science New([NotNull]string name, [NotNull]IPublisher publisher)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            return new Science(name, publisher);
        }
    }
}
