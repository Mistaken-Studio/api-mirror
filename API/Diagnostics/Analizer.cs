// -----------------------------------------------------------------------
// <copyright file="Analizer.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace Mistaken.API.Diagnostics
{
    internal static class Analizer
    {
        internal static void AnalizeContent(string file)
        {
            if (!File.Exists(file))
                return;
            var result = AnalizeContent(File.ReadAllLines(file));
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".analized.raw.log"), Newtonsoft.Json.JsonConvert.SerializeObject(result));
            File.Delete(file);
        }

        private static Dictionary<string, Data> AnalizeContent(string[] lines)
        {
            Dictionary<string, List<(float Took, DateTime Time)>> times = new Dictionary<string, List<(float Took, DateTime Time)>>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string[] data = line.Split('|');
                var time = new DateTime(long.Parse(data[0]));
                string executor = string.Join(".", data[1].Trim().Replace(" ", "_").Split(new string[] { ":" }, StringSplitOptions.None));
                float timeTook = float.Parse(data[2]);
                if (!times.ContainsKey(executor))
                    times.Add(executor, new List<(float Took, DateTime Time)>());
                times[executor].Add((timeTook, time));
            }

            Dictionary<string, Data> proccesedData = new Dictionary<string, Data>();
            foreach (var time in times)
            {
                float min = float.MaxValue;
                float max = 0;
                float avg = 0;
                Dictionary<string, int> calls = new Dictionary<string, int>();
                foreach (var (took, time1) in time.Value)
                {
                    avg += took;
                    if (max < took)
                        max = took;
                    if (min > took)
                        min = took;
                    string stringTime = time1.ToString("yyyy-MM-dd HH-mm");
                    if (!calls.ContainsKey(stringTime))
                        calls.Add(stringTime, 0);
                    calls[stringTime]++;
                }

                float avgCalls = 0;
                foreach (var item in calls)
                    avgCalls += item.Value;
                avgCalls /= calls.Values.Count;
                avg /= time.Value.Count;
                var info = (avg, time.Value.Count, min, max, avgCalls);
                proccesedData.Add(time.Key, new Data(info));
            }

            return proccesedData;
        }

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
