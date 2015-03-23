using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Shience
{
    public class Science<TResult>
    {
        private readonly string _testName;
        private readonly IComparer<TResult> _comparer;
        private const string PublishFilePath = @"D:\scienceResults.txt";

        internal Science(string testName)
        {
            _testName = testName;
        }

        internal Science(string testName, IComparer<TResult> comparer)
        {
            _testName = testName;
            _comparer = comparer;
        }

        public TResult Test(Func<TResult> control, Func<TResult> candidate)
        {
            //If candidate is null, don't do any science
            if (candidate == null)
            {
                return Run(control);
            }

            var experimentResult = new ExperimentResult<TResult>(_comparer);
            experimentResult.TestName = _testName;

            TestResult<TResult> controlResult, candidateResult;

            if (new Random().Next()%2 == 0)
            {
                experimentResult.ControlRanFirst = true;
                controlResult = InternalTest(control);
                candidateResult = InternalTest(candidate);
            }
            else
            {
                candidateResult = InternalTest(candidate);
                controlResult = InternalTest(control);
            }

            experimentResult.ControlResult = controlResult;
            experimentResult.CandidateResult = candidateResult;

            Publish(experimentResult);

            return controlResult.Result;
        }

        private TestResult<TResult> InternalTest(Func<TResult> action)
        {
            var tr = new TestResult<TResult>();
            var sw = new Stopwatch();

            sw.Start();

            try
            {
                var result = Run(action);
                tr.Result = result;
            }
            catch (Exception e)
            {
                tr.Exception = e;
            }

            sw.Stop();

            tr.RunTime = sw.ElapsedMilliseconds;
            return tr;
        }

        private TResult Run(Func<TResult> action)
        {
            return action();
        }

        public void Publish(ExperimentResult<TResult> result)
        {
            using (var sw = new StreamWriter(PublishFilePath, true))
            {
                sw.WriteLine(result.TestName + "|" + result.ControlResult.Result + "|" + result.ControlResult.RunTime + "|" + result.CandidateResult.Result + "|" + result.CandidateResult.RunTime + "|" + result.Matched);
            }
        }
    }
}
