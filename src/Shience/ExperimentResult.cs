namespace Shience
{
    public class ExperimentResult<TResult>
    {
        public string TestName { get; internal set; }
        public bool ControlRanFirst { get; internal set; }
        public dynamic Context { get; internal set; }
        
        public TestResult<TResult> ControlResult { get; internal set; }
        public TestResult<TResult> CandidateResult { get; internal set; }
        public bool Matched { get; internal set; }
    }
}
