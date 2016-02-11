using System;
using JetBrains.Annotations;

namespace Shience
{
    public static class Shience
    {
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
