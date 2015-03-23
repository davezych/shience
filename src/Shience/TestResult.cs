using System;

namespace Shience
{
    public class TestResult<TResult>
    {
        public TResult Result { get; set; }
        public Exception Exception { get; set; }
        public long RunTime { get; set; }
    }
}
