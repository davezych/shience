using System;
using System.IO;
using System.Text;
using Shience.Result;

namespace Shience.Publish
{
    public class FilePublisher : IPublisher
    {
        private readonly string _filePath;

        public FilePublisher(string path)
        {
            _filePath = path;
        } 

        public void Publish<TResult>(ExperimentResult<TResult> result)
        {
            var sb = new StringBuilder();

            sb.Append(DateTime.UtcNow);
            sb.Append("|");
            sb.Append(result.TestName);
            sb.Append("|");
            sb.Append(result.ControlResult.Result);
            sb.Append("|");
            sb.Append(result.ControlResult.RunTime);
            sb.Append("|");
            sb.Append(result.CandidateResult.Result);
            sb.Append("|");
            sb.Append(result.CandidateResult.RunTime);
            sb.Append("|");
            sb.Append(result.Matched);
            sb.Append("|");
            sb.Append(result.Contexts);
            sb.Append("|");
            sb.Append(result.ControlResult.Exception);
            sb.Append("|");
            sb.Append(result.CandidateResult.Exception);
            sb.Append("|");
            sb.Append(string.Join(";", result.Contexts));
            
            using (var sw = new StreamWriter(_filePath, true))
            {
                sw.WriteLine(sb.ToString());
            }
        }
    }
}