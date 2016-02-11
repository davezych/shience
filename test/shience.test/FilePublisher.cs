using JetBrains.Annotations;
using Jil;
using System;
using System.IO;

namespace Shience.Publish
{
    public sealed class FilePublisher
    {
        private string FilePath { get; }

        public FilePublisher([NotNull]string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            FilePath = path;
        }

        public void Publish<TResult>([NotNull]ExperimentResult<TResult> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            var json = ConvertToJson(result);

            using (var fs = new FileStream(FilePath, FileMode.Append))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(json);
            }
        }

        private static string ConvertToJson<TResult>(ExperimentResult<TResult> result)
        {
            var @object = new
            {
                Timestamp = DateTime.UtcNow,
                Name = result.TestName,
                ControlResult = result.ControlResult.Result.ToString(),
                ControlRunTime = result.ControlResult.RunTime,
                CandidateResult = result.CandidateResult.Result.ToString(),
                CandidateRunTime = result.CandidateResult.RunTime,
                result.Matched,
                Exception = result.ControlResult.Exception,
                result.Context
            };

            return Serialize(@object);
        }

        private static string Serialize(dynamic @object)
        {
            using (var output = new StringWriter())
            {
                JSON.Serialize(@object, output);
                return output.ToString();
            }
        }

    }
}