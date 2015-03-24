namespace Shience.Result
{
    public class ExperimentResult<TResult>
    {
        public string TestName { get; set; }
        public TestResult<TResult> ControlResult { get; set; }
        public TestResult<TResult> CandidateResult { get; set; }
        public bool ControlRanFirst { get; set; }

        public bool Matched
        {
            get { return ControlResult.Result.Equals(CandidateResult.Result); }
        }
    }
}
