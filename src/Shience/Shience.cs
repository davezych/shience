using System.Collections.Generic;

namespace Shience
{
    public static class Shience
    {
        public static Science<TResult> New<TResult>(string name)
        {
            return new Science<TResult>(name);
        }

        public static Science<TResult> New<TResult>(string name, IComparer<TResult> comparer)
        {
            return new Science<TResult>(name, comparer);
        }
    }
}
