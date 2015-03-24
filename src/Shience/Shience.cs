using System;
using Shience.Publish;

namespace Shience
{
    public static class Shience
    {
        private static Type _publisherType;
        private static object[] _publisherArgs;

        public static void SetPublisher(Type publisherType, params object[] args)
        {
            _publisherType = publisherType;
            _publisherArgs = args;
        }

        private static IPublisher<T> GetInstanceOfPublisher<T>()
        {
            if (_publisherType == null)
            {
                throw new ArgumentException("PublisherType");
            }

            var constructed = _publisherType.MakeGenericType(typeof (T));
            return (IPublisher<T>) Activator.CreateInstance(constructed, _publisherArgs);
        }

        public static Science<TResult> New<TResult>(string name)
        {
            return new Science<TResult>(name, GetInstanceOfPublisher<TResult>());
        }
    }
}
