namespace Shience
{
    public class ExperimentResult<TControlResult, TCandidateResult>
    {
        public string TestName { get; internal set; }
        public bool ControlRanFirst { get; internal set; }
        public dynamic Context { get; internal set; }
        
        public TestResult<TControlResult> ControlResult { get; internal set; }
        public TestResult<TCandidateResult> CandidateResult { get; internal set; }
        public bool Matched { get; internal set; }
    }
}
