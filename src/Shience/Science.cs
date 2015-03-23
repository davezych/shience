using System;

namespace Shience
{
    public class Science<TResult>
    {
        public TResult Test(Func<TResult> control, Func<TResult> candidate)
        {
            var controlResult = control();
            var candidateResult = candidate();

            return controlResult;
        }
    }
}
