using System;

namespace Shience
{
    public class TestResult<TResult>
    {
        public TResult Result { get; internal set; }
        public Exception Exception { get; internal set; }
        public DateTime StartTimeUtc { get; internal set; }
        public TimeSpan RunTime { get; internal set; }
    }
}
