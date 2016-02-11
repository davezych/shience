using System;
using System.IO;
using System.Text;

namespace Shience.Test.Publishers
{
    public class FilePublisher
    {
        private readonly string _path;

        public FilePublisher(string path)
        {
            _path = path;
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

            using (var fs = new FileStream(_path, FileMode.Append))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(sb);
            }
        }
    }
}