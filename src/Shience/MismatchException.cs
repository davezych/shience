using System;

namespace Shience
{
    public class MismatchException : Exception
    {
        public MismatchException()
        {
        }

        public MismatchException(string message) : base(message)
        {
        }

        public MismatchException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}