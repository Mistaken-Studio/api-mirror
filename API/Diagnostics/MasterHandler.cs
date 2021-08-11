// -----------------------------------------------------------------------
// <copyright file="MasterHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Exiled.API.Features;
using MEC;

namespace Mistaken.API.Diagnostics
{
    /// <summary>
    /// Master module handler.
    /// </summary>
    public static partial class MasterHandler
    {
        /// <summary>
        /// Handlers bound to Module.
        /// </summary>
        public static readonly Dictionary<Module, Dictionary<string, Exiled.Events.Events.CustomEventHandler>> Handlers = new Dictionary<Module, Dictionary<string, Exiled.Events.Events.CustomEventHandler>>();

        /// <summary>
        /// Called when module throws error when handling event.
        /// </summary>
        public static event Action<System.Exception, Module, string> OnError;

        /// <summary>
        /// Logs Error.
        /// </summary>
        /// <param name="ex">Catched exception.</param>
        /// <param name="module">Catching module.</param>
        /// <param name="name">Catching function name.</param>
        public static void LogError(System.Exception ex, Module module, string name)
        {
            ErrorBacklog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [{module?.Name ?? "Rouge"}: {name}] Caused Exception");
            ErrorBacklog.Add(ex.Message);
            ErrorBacklog.Add(ex.StackTrace);
            if (!CI_TEST_SERVER_PORTS.Contains(Server.Port))
                return;
            CurrentStatus.StatusCode = 1;
            CurrentStatus.Exceptions.Add(new Exception
            {
                Ex = ex,
                Module = module,
                Name = name,
            });
            File.WriteAllText(Path.Combine(Paths.Exiled, "RunResult.txt"), Newtonsoft.Json.JsonConvert.SerializeObject(CurrentStatus));
        }

        /// <summary>
        /// Handles event and mesures time that took to handle.
        /// </summary>
        /// <param name="module">Modue.</param>
        /// <param name="action">Handler.</param>
        /// <param name="name">Handler Name.</param>
        /// <returns>Event Handler.</returns>
        public static Exiled.Events.Events.CustomEventHandler Handle(this Module module, Action action, string name)
        {
            if (!Handlers.ContainsKey(module))
                Handlers[module] = new Dictionary<string, Exiled.Events.Events.CustomEventHandler>();
            if (Handlers[module].ContainsKey(name))
                return Handlers[module][name];
            DateTime start;
            DateTime end;
            TimeSpan diff;
            void Tor()
            {
                start = DateTime.Now;
                try
                {
                    action();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"[{DateTime.Now:HH:mm:ss.fff}] [{module.Name}: {name}] Caused Exception");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                    LogError(ex, module, name);

                    // ErrorBacklog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [{module.Name}: {name}] Caused Exception");
                    // ErrorBacklog.Add(ex.Message);
                    // ErrorBacklog.Add(ex.StackTrace);
                    OnError?.Invoke(ex, module, name);
                }

                end = DateTime.Now;
                diff = end - start;
                Backlog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [{module.Name}: {name}] {diff.TotalMilliseconds}");
            }

            Handlers[module][name] = Tor;
            return Tor;
        }

        /// <summary>
        /// Handles event and mesures time that took to handle.
        /// </summary>
        /// <typeparam name="T">Event Args Type.</typeparam>
        /// <param name="module">Modue.</param>
        /// <param name="action">Handler.</param>
        /// <returns>Event Handler.</returns>
        public static Exiled.Events.Events.CustomEventHandler<T> Handle<T>(this Module module, Action<T> action)
            where T : EventArgs
            => Generic<T>.Handle(module, action);

        /// <summary>
        /// Logs Time.
        /// </summary>
        /// <param name="moduleName">Module Name.</param>
        /// <param name="name">Handler Name.</param>
        /// <param name="start">Handling start time.</param>
        /// <param name="end">Handling end time.</param>
        public static void LogTime(string moduleName, string name, DateTime start, DateTime end) =>
            Backlog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [{moduleName}: {name}] {(end - start).TotalMilliseconds}");

        internal static Status CurrentStatus { get; set; } = new Status();

        internal static void Ini()
        {
            Log.Debug($"Called Ini", PluginHandler.Instance.Config.VerbouseOutput);
            if (initiated)
                return;
            if (CI_TEST_SERVER_PORTS.Contains(Server.Port))
            {
                CurrentStatus = new Status();
                File.WriteAllText(Path.Combine(Paths.Exiled, "RunResult.txt"), Newtonsoft.Json.JsonConvert.SerializeObject(CurrentStatus));
            }

            Timing.RunCoroutine(SaveLoop());
            initiated = true;
        }

        private static readonly List<string> Backlog = new List<string>();
        private static readonly List<string> ErrorBacklog = new List<string>();
        private static readonly ushort[] CI_TEST_SERVER_PORTS = new ushort[] { 8050, 8008 };
        private static bool initiated = false;

        private static IEnumerator<float> SaveLoop()
        {
            Log.Debug($"Starting Loop", PluginHandler.Instance.Config.VerbouseOutput);
            if (!Directory.Exists($"{Paths.Configs}/{Server.Port}/"))
            {
                Directory.CreateDirectory($"{Paths.Configs}/{Server.Port}/");
                Log.Debug($"{Paths.Configs}/{Server.Port}/ Created", PluginHandler.Instance.Config.VerbouseOutput);
            }
            else
            {
                Log.Debug($"{Paths.Configs}/{Server.Port}/ Exists", PluginHandler.Instance.Config.VerbouseOutput);
            }

            string lastDay = DateTime.Now.ToString("yyyy-MM-dd");
            string day;
            while (true)
            {
                try
                {
                    day = DateTime.Now.ToString("yyyy-MM-dd");
                    if (lastDay != day)
                    {
                        Compress($"{Paths.Configs}/{Server.Port}/{lastDay}");
                        lastDay = day;
                    }

                    if (!Directory.Exists($"{Paths.Configs}/{Server.Port}/{day}/"))
                    {
                        Directory.CreateDirectory($"{Paths.Configs}/{Server.Port}/{day}/");
                        Log.Debug($"Created {Paths.Configs}/{Server.Port}/{day}/", PluginHandler.Instance.Config.VerbouseOutput);
                    }

                    // Log.Debug($"{Paths.Configs}/{Server.Port}/{day}/{DateTime.Now.ToString("yyyy-MM-dd_HH")}.log");
                    string path = $"{Paths.Configs}/{Server.Port}/{day}/{DateTime.Now:yyyy-MM-dd_HH}.log";
                    if (!File.Exists(path))
                        AnalizeContent($"{Paths.Configs}/{Server.Port}/{day}/{DateTime.Now.AddHours(-1):yyyy-MM-dd_HH}.log");
                    File.AppendAllLines(path, Backlog);
                    Backlog.Clear();
                    File.AppendAllLines($"{Paths.Configs}/{Server.Port}/{day}/error.log", ErrorBacklog);
                    ErrorBacklog.Clear();
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                yield return MEC.Timing.WaitForSeconds(1);
            }
        }

        private static void Compress(string day)
        {
            try
            {
                ZipFile.CreateFromDirectory(day, $"{day}.zip");
                Directory.Delete(day, true);
            }
            catch (System.Exception ex)
            {
                Log.Error("Failed to compress");
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }

        private static void AnalizeContent(string file)
        {
            if (!File.Exists(file))
                return;
            var result = AnalizeContent(File.ReadAllLines(file), DateTime.Now.AddHours(-1));
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".analized.raw.log"), Newtonsoft.Json.JsonConvert.SerializeObject(result));
            File.Delete(file);
        }

        private static Dictionary<string, Data> AnalizeContent(string[] lines, DateTime dateTime)
        {
            Dictionary<string, List<(float Took, DateTime Time)>> times = new Dictionary<string, List<(float Took, DateTime Time)>>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string[] data = line.Replace("[", string.Empty).Split(']');
                string[] date = data[0].Split(':');
                var time = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2].Split('.')[0]), int.Parse(date[2].Split('.')[1]));
                string executor = string.Join(".", data[1].Trim().Replace(" ", string.Empty).Split(new string[] { ":" }, StringSplitOptions.None));
                data[2] = data[2].Replace(".", ",");
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

        private class Data
        {
            public Data((float Avg, int Calls, float Min, float Max, float AvgCallsPerMinute) info)
            {
                this.avg = info.Avg;
                this.calls = info.Calls;
                this.min = info.Min;
                this.max = info.Max;
                this.avgCallsPerMinute = info.AvgCallsPerMinute;
            }

#pragma warning disable IDE0052 // Usuń nieodczytywane składowe prywatne
            private readonly float avg;
            private readonly int calls;
            private readonly float min;
            private readonly float max;
            private readonly float avgCallsPerMinute;
#pragma warning restore IDE0052 // Usuń nieodczytywane składowe prywatne
        }
    }
}
