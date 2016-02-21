using System;
using System.Collections.Generic;

namespace Shience
{
    public class TestResult<TResult> : IEquatable<TestResult<TResult>>
    {
        public TResult Result { get; internal set; }
        public Exception Exception { get; internal set; }
        public DateTime StartTimeUtc { get; internal set; }
        public TimeSpan RunTime { get; internal set; }

        #region IEquatable
        public bool Equals(TestResult<TResult> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TResult>.Default.Equals(Result, other.Result) && Equals(Exception, other.Exception);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TestResult<TResult>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TResult>.Default.GetHashCode(Result)*397) ^ (Exception?.GetHashCode() ?? 0);
            }
        }
        #endregion
    }
}
