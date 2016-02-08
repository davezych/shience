﻿using System;
using System.Collections.Generic;

namespace Shience
{
    public class ExperimentResult<TResult>
    {
        public string TestName { get; set; }
        public TestResult<TResult> ControlResult { get; set; }
        public TestResult<TResult> CandidateResult { get; set; }
        public bool ControlRanFirst { get; set; }
        public List<object> Contexts { get; set; }
        public Func<TResult, TResult, bool> ComparerFunc { get; set; }

        public ExperimentResult()
        {
            Contexts = new List<object>();
        }

        public bool Matched => ComparerFunc?.Invoke(ControlResult.Result, CandidateResult.Result) 
            ?? ControlResult.Result.Equals(CandidateResult.Result);
    }
}