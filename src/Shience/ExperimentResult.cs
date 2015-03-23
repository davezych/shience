using System.Collections.Generic;

namespace Shience
{
    public class ExperimentResult<TResult>
    {
        public string TestName { get; set; }
        public TestResult<TResult> ControlResult { get; set; }
        public TestResult<TResult> CandidateResult { get; set; }
        private readonly IComparer<TResult> _comparer;

        public ExperimentResult()
        {
            _comparer = Comparer<TResult>.Default;
        }
        
        public ExperimentResult(IComparer<TResult> comparer)
        {
            _comparer = comparer;
        }

        public bool Matched
        {
            get { return _comparer.Compare(ControlResult.Result, CandidateResult.Result) == 0; }
        }
    }
}
