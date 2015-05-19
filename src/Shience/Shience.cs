using Shience.Publish;

namespace Shience
{
    public static class Shience
    {
        private static IPublisher _publisher;

        public static void SetPublisher(IPublisher publisher)
        {
            _publisher = publisher;
        }
        
        public static Science<TResult> New<TResult>(string name)
        {
            return new Science<TResult>(name, _publisher);
        }
    }
}
