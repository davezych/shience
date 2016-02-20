using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Shience
{
    internal static class FuncExtensions
    {
        internal static async Task<TestResult<TResult>> InternalTestAsync<TResult>([NotNull]this Func<TResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var testResult = new TestResult<TResult>();
            var timer = new Stopwatch();
            testResult.StartTimeUtc = DateTime.UtcNow;
            timer.Start();
            try
            {
                var result = RunAsync(action);
                testResult.Result = await result;
            }
            catch (Exception e)
            {
                testResult.Exception = e;
            }
            finally
            {
                timer.Stop();
            }
            
            testResult.RunTime = timer.Elapsed;
            return testResult;
        }

        internal static async Task<TResult> RunAsync<TResult>([NotNull]this Func<TResult> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var result = await Task.Run(action);
            return result;
        }
    }
}