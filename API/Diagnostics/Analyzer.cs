// -----------------------------------------------------------------------
// <copyright file="Analyzer.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace Mistaken.API.Diagnostics
{
    internal static class Analyzer
    {
        internal static void AnalyzeContent(string file)
        {
            if (!File.Exists(file))
                return;

            var result = AnalyzeContent(File.ReadAllLines(file));
            File.WriteAllText(
                Path.Combine(
                    Path.GetDirectoryName(file)!,
                    Path.GetFileNameWithoutExtension(file) + ".analized.raw.log"),
                Newtonsoft.Json.JsonConvert.SerializeObject(result));
            File.Delete(file);
        }

        private static Dictionary<string, Data> AnalyzeContent(string[] lines)
        {
            Dictionary<string, List<(float Took, DateTime Time)>> times = new();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var data = line.Split('|');
                var time = new DateTime(long.Parse(data[0]));
                var executor = string.Join(".", data[1].Trim().Replace(" ", "_").Split(new[] { ":" }, StringSplitOptions.None));
                var timeTook = float.Parse(data[2]);
                if (!times.ContainsKey(executor))
                    times.Add(executor, new List<(float Took, DateTime Time)>());
                times[executor].Add((timeTook, time));
            }

            Dictionary<string, Data> processedData = new();
            foreach (var time in times)
            {
                var min = float.MaxValue;
                float max = 0;
                float avg = 0;
                foreach (var (took, _) in time.Value)
                {
                    avg += took;
                    if (max < took)
                        max = took;
                    if (min > took)
                        min = took;
                }

                var avgCalls = time.Value.Count / 60f;
                avg /= time.Value.Count;
                var info = (avg, time.Value.Count, min, max, avgCalls);
                processedData.Add(time.Key, new Data(info));
            }

            return processedData;
        }

        [PublicAPI("Used for Json")]
        private struct Data
        {
            public Data((float Avg, int Calls, float Min, float Max, float AvgCallsPerMinute) info)
            {
                this.Avg = info.Avg;
                this.Calls = info.Calls;
                this.Min = info.Min;
                this.Max = info.Max;
                this.AvgCallsPerMinute = info.AvgCallsPerMinute;
            }

            public float Avg { get; }

            public int Calls { get; }

            public float Min { get; }

            public float Max { get; }

            public float AvgCallsPerMinute { get; }
        }
    }
}
