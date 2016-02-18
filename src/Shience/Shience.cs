using System;
using JetBrains.Annotations;

namespace Shience
{
    public static class Shience
    {
        public static Experiment<TResult> New<TResult>([NotNull]string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            
            return new Experiment<TResult>(name);
        }
    }
}
