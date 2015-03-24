using System;
using Shience.Publish;

namespace Shience
{
    public static class Shience
    {
        private static Type _publisherType;

        public static void SetPublisher(Type publisherType)
        {
            _publisherType = publisherType;
        }

        private static IPublisher<T> GetInstanceOfPublisher<T>()
        {
            if (_publisherType == null)
            {
                throw new ArgumentException("PublisherType");
            }

            var constructed = _publisherType.MakeGenericType(typeof (T));
            return (IPublisher<T>) Activator.CreateInstance(constructed);
        }

        public static Science<TResult> New<TResult>(string name)
        {
            return new Science<TResult>(name, GetInstanceOfPublisher<TResult>());
        }
    }
}
