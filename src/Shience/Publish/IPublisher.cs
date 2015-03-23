using Shience.Result;

namespace Shience.Publish
{
    public interface IPublisher<TResult>
    {
        void Publish(ExperimentResult<TResult> result);
    }
}