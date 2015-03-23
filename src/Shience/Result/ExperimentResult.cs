using System.Collections.Generic;

namespace Shience.Result
{
    public class ExperimentResult<TResult>
    {
        public string TestName { get; set; }
        public TestResult<TResult> ControlResult { get; set; }
        public TestResult<TResult> CandidateResult { get; set; }
        private readonly IComparer<TResult> _comparer;
        public bool ControlRanFirst { get; set; }

        public ExperimentResult(IComparer<TResult> comparer)
        {
            if (comparer == null)
            {
                comparer = Comparer<TResult>.Default;
            }
            _comparer = comparer;
        }

        public bool Matched
        {
            get { return _comparer.Compare(ControlResult.Result, CandidateResult.Result) == 0; }
        }
    }
}
