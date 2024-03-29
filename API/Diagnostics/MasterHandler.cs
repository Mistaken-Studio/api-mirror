// -----------------------------------------------------------------------
// <copyright file="MasterHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using JetBrains.Annotations;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162

namespace Mistaken.API.Diagnostics
{
    /// <summary>
    /// Master module handler.
    /// </summary>
    [PublicAPI]
    public static partial class MasterHandler
    {
        /// <summary>
        /// If diagnostics data should be logged (Exceptions will still be logged and catched).
        /// </summary>
        public const bool DiagnosticsEnabled = false;

        /// <summary>
        /// Called when module throws error when handling event.
        /// </summary>
        public static event Action<System.Exception, string> OnErrorCatched;

        /// <summary>
        /// Called when module throws error when handling event.
        /// </summary>
        public static event Action<string, string> OnUnityCatchedException;

        /// <summary>
        /// Logs Error.
        /// </summary>
        /// <param name="ex">Catched exception.</param>
        /// <param name="module">Catching module.</param>
        /// <param name="name">Catching function name.</param>
        public static void LogError(System.Exception ex, Module module, string name)
        {
            LogError(ex, $"{module?.Name ?? "Rouge"}: {name}");
            if (!PluginHandler.Instance.Config.GenerateRunResultFile)
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
        /// Logs Time.
        /// </summary>
        /// <param name="moduleName">Module Name.</param>
        /// <param name="name">Handler Name.</param>
        /// <param name="start">Handling start time.</param>
        /// <param name="end">Handling end time.</param>
        public static void LogTime(string moduleName, string name, DateTime start, DateTime end) =>
            LogTime(moduleName + ": " + name, (end - start).TotalMilliseconds);

        internal static Status CurrentStatus { get; private set; } = new();

        internal static void LogError(System.Exception ex, string method)
        {
            lock (ErrorBacklogLockObj)
            {
                ErrorBacklog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [{method}] Caused Exception");
                ErrorBacklog.Add(ex.ToString());
            }

            OnErrorCatched?.Invoke(ex, method);
        }

        internal static void LogJunk(string name)
        {
            fileStream ??= File.OpenWrite(
                Path.Combine(
                    Paths.Plugins,
                    "Diagnostics",
                    "Junk",
                    Server.Port.ToString(),
                    DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + ".log"));

            var tow = Encoding.UTF8.GetBytes($"[{DateTime.Now:HH:mm:ss.fff}] {name}\n");
            fileStream.Write(tow, 0, tow.Length);
            fileStream.Flush(true);
        }

        internal static void LogTime(string name, double time)
        {
            if (!DiagnosticsEnabled)
                return;

            lock (BacklogLockObj)
                Backlog.Add(new Entry(name, time));
        }

        internal static void Ini()
        {
            Log.Debug("Called Ini", PluginHandler.Debug);
            if (initiated)
                return;
            if (PluginHandler.Instance.Config.GenerateRunResultFile)
            {
                CurrentStatus = new Status();
                File.WriteAllText(Path.Combine(Paths.Exiled, "RunResult.txt"), Newtonsoft.Json.JsonConvert.SerializeObject(CurrentStatus));
            }

            if (DiagnosticsEnabled)
                LiteNetLib4MirrorNetworkManager.singleton.gameObject.AddComponent<DeltaTimeChecker>();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.logMessageReceived += Application_logMessageReceived;

            initiated = true;

            if (DiagnosticsEnabled && false)
                _ = SaveLoop();
        }

        private static readonly object BacklogLockObj = new();
        private static readonly object ErrorBacklogLockObj = new();
        private static readonly List<Entry> Backlog = new();
        private static readonly List<string> ErrorBacklog = new();
        private static bool initiated;

        private static FileStream fileStream;

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError(e.ExceptionObject as System.Exception, "UnhandledException");
            Log.Error($"Detected UnhandledException, Is Terminating: {e.IsTerminating}");
            Log.Error(e.ExceptionObject);
        }

        private static void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception && type != LogType.Error)
            {
                if (type == LogType.Assert)
                    Log.Debug($"[DIAGNOSTICS] Skipped {type}, {condition}");
                return;
            }

            OnUnityCatchedException?.Invoke(condition, stackTrace);

            lock (ErrorBacklogLockObj)
            {
                ErrorBacklog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [Application_logMessageReceived] Caused Exception");
                ErrorBacklog.Add(condition + "\n" + stackTrace);
            }

            Log.Error($"Detected Unity LogMessage of typ Exception");
            Log.Error(condition + "\n" + stackTrace);
        }

        private static async Task SaveLoop()
        {
            var path = Path.Combine(Paths.Plugins, "Diagnostics");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Log.Debug($"{path} Created", PluginHandler.Debug);
            }
            else
            {
                Log.Debug($"{path} Exists", PluginHandler.Debug);
            }

            path = Path.Combine(path, Server.Port.ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Log.Debug($"{path} Created", PluginHandler.Debug);
            }
            else
            {
                Log.Debug($"{path} Exists", PluginHandler.Debug);
            }

            var now = DateTime.Now;

            var lastDay = now.ToString("yyyy-MM-dd");
            string day;
            string internalPath;

            Log.Debug($"Starting Loop", PluginHandler.Debug);
            while (initiated)
            {
                now = DateTime.Now;
                internalPath = path;
                try
                {
                    day = now.ToString("yyyy-MM-dd");

                    internalPath = Path.Combine(internalPath, day);

                    if (!Directory.Exists(internalPath))
                    {
                        Directory.CreateDirectory(internalPath);
                        Log.Debug($"Created {internalPath}", PluginHandler.Debug);
                    }

                    // Log.Debug($"{Paths.Configs}/{Server.Port}/{day}/{DateTime.Now.ToString("yyyy-MM-dd_HH")}.log");
                    var filePath = Path.Combine(internalPath, $"{now:yyyy-MM-dd_HH}.log");
                    if (!File.Exists(filePath))
                    {
                        if (now.Hour == 0)
                            Analyzer.AnalyzeContent(Path.Combine(path, lastDay, $"{now.AddDays(-1):yyyy-MM-dd}_23.log"));
                        else
                            Analyzer.AnalyzeContent(Path.Combine(internalPath, $"{now.AddHours(-1):yyyy-MM-dd_HH}.log"));
                    }

                    if (lastDay != day)
                    {
                        Compress(Path.Combine(path, lastDay));
                        lastDay = day;
                    }

                    lock (BacklogLockObj)
                    {
                        try
                        {
                            File.AppendAllLines(filePath, Backlog.Select(x => x.ToString()).ToArray());
                            Backlog.Clear();
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }

                    lock (ErrorBacklogLockObj)
                    {
                        try
                        {
                            File.AppendAllLines(Path.Combine(internalPath, "error.log"), ErrorBacklog);
                            ErrorBacklog.Clear();
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }

                await Task.Delay(1000);
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
                Log.Error(ex);
            }
        }
    }
}
