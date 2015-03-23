using System.Collections.Generic;
using Shience.Publish;

namespace Shience
{
    public static class Shience
    {
        public static Science<TResult> New<TResult>(string name)
        {
            var publisher = new FilePublisher<TResult>(@"D:\shienceResults.txt");
            return new Science<TResult>(name, publisher);
        }

        public static Science<TResult> New<TResult>(string name, IComparer<TResult> comparer)
        {
            var publisher = new FilePublisher<TResult>(@"D:\shienceResults.txt");
            return new Science<TResult>(name, publisher, comparer);
        }
    }
}
