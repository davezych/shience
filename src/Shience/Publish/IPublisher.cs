namespace Shience.Publish
{
    public interface IPublisher
    {
        void Publish<TResult>(ExperimentResult<TResult> result);
    }
}