﻿using System;

namespace Shience
{
    public class ExperimentResult<TResult>
    {
        public string TestName { get; set; }
        public TestResult<TResult> ControlResult { get; set; }
        public TestResult<TResult> CandidateResult { get; set; }
        public bool ControlRanFirst { get; set; }
        public dynamic Contexts { get; set; }
        public Func<TResult, TResult, bool> ComparerFunc { get; set; }
        
        public bool Matched => ComparerFunc?.Invoke(ControlResult.Result, CandidateResult.Result) 
            ?? ControlResult.Result.Equals(CandidateResult.Result);
    }
}
