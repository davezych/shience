﻿using System;
using System.IO;
using JetBrains.Annotations;
using Jil;

namespace Shience.Test
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

        public void Publish<TControl, TCandidate>([NotNull]ExperimentResult<TControl, TCandidate> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            var json = ConvertToJson(result);

            using (var fs = new FileStream(FilePath, FileMode.Append))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(json);
            }
        }

        private static string ConvertToJson<TControl, TCandidate>(ExperimentResult<TControl, TCandidate> result)
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
                result.ControlResult.Exception,
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