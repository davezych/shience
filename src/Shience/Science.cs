using System;
using JetBrains.Annotations;

namespace Shience
{
    public static class Science
    {
        public static Experiment<TResult, TResult> New<TResult>([NotNull]string name)
        {
            return New<TResult, TResult>(name);
        }

        public static Experiment<TControlResult, TCandidateResult> New<TControlResult, TCandidateResult>([NotNull]string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            
            return new Experiment<TControlResult, TCandidateResult>(name);
        }
    }
}
