﻿using System.IO;
using Shience.Result;

namespace Shience.Publish
{
    public class FilePublisher<TResult> : IPublisher<TResult>
    {
        private const string PublishFilePath = @"D:\scienceResults.txt";

        public void Publish(ExperimentResult<TResult> result)
        {
            using (var sw = new StreamWriter(PublishFilePath, true))
            {
                //TODO: Need to add exception (if any) to publishing
                sw.WriteLine(result.TestName + "|" + result.ControlResult.Result + "|" + result.ControlResult.RunTime + "|" + result.CandidateResult.Result + "|" + result.CandidateResult.RunTime + "|" + result.Matched);
            }
        }
    }
}