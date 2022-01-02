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
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;

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
        [System.Obsolete("Will removed because of change in Handlers")]
        public static readonly Dictionary<Module, Dictionary<string, Exiled.Events.Events.CustomEventHandler>> Handlers = new Dictionary<Module, Dictionary<string, Exiled.Events.Events.CustomEventHandler>>();

        /// <inheritdoc cref="OnErrorCatched"/>
        [System.Obsolete("Use OnErrorCatched", true)]
#pragma warning disable CS0067
        public static event Action<System.Exception, Module, string> OnError;
#pragma warning restore CS0067

        /// <summary>
        /// Called when module throws error when handling event.
        /// </summary>
        public static event Action<System.Exception, string> OnErrorCatched;

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
        /// Handles event and mesures time that took to handle.
        /// </summary>
        /// <param name="module">Modue.</param>
        /// <param name="action">Handler.</param>
        /// <param name="name">Handler Name.</param>
        /// <returns>Event Handler.</returns>
        [System.Obsolete("Functionality of diagnostics system was moved to exiled.events system, subscribe event directly")]
        public static Exiled.Events.Events.CustomEventHandler Handle(this Module module, Action action, string name)
        {
            if (!Handlers.ContainsKey(module))
                Handlers[module] = new Dictionary<string, Exiled.Events.Events.CustomEventHandler>();
            if (Handlers[module].ContainsKey(name))
                return Handlers[module][name];

            Exiled.Events.Events.CustomEventHandler tor = new Exiled.Events.Events.CustomEventHandler(action);

            Handlers[module][name] = tor;
            if (PluginHandler.Instance.Config.VerbouseOutput)
                Log.Warn(module.Name + " called obsolete function Handle");
            return tor;
        }

        /// <summary>
        /// Handles event and mesures time that took to handle.
        /// </summary>
        /// <typeparam name="T">Event Args Type.</typeparam>
        /// <param name="module">Modue.</param>
        /// <param name="action">Handler.</param>
        /// <returns>Event Handler.</returns>
        [System.Obsolete("Functionality of diagnostics system was moved to exiled.events system, subscribe event directly")]
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
            LogTime(moduleName + ": " + name, (end - start).TotalMilliseconds);

        internal static Status CurrentStatus { get; set; } = new Status();

        internal static void LogError(System.Exception ex, string method)
        {
            ErrorBacklog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [{method}] Caused Exception");
            ErrorBacklog.Add(ex.ToString());

            OnErrorCatched?.Invoke(ex, method);
        }

        internal static void LogTime(string name, double time) =>
            Backlog.Add(new Entry(name, time));

        internal static void Ini()
        {
            Log.Debug($"Called Ini", PluginHandler.Instance.Config.VerbouseOutput);
            if (initiated)
                return;
            if (PluginHandler.Instance.Config.GenerateRunResultFile)
            {
                CurrentStatus = new Status();
                File.WriteAllText(Path.Combine(Paths.Exiled, "RunResult.txt"), Newtonsoft.Json.JsonConvert.SerializeObject(CurrentStatus));
            }

            CustomNetworkManager.singleton.gameObject.AddComponent<DeltaTimeChecker>();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.logMessageReceived += Application_logMessageReceived;

            _ = SaveLoop();
            initiated = true;
        }

        private static void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception)
            {
                Log.Debug($"[DIAGNOSTICS] Skipped {type}, {condition}");
                return;
            }

            ErrorBacklog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [Application_logMessageReceived] Caused Exception");
            ErrorBacklog.Add(condition + "\n" + stackTrace);
            Log.Error($"Detected Unity LogMessage of typ Exception");
            Log.Error(condition + "\n" + stackTrace);
        }

        private static readonly List<Entry> Backlog = new List<Entry>();
        private static readonly List<string> ErrorBacklog = new List<string>();
        private static bool initiated = false;

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError(e.ExceptionObject as System.Exception, "UnhandledException");
            Log.Error($"Detected UnhandledException, Is Terminating: {e.IsTerminating}");
            Log.Error(e.ExceptionObject);
        }

        private static async Task SaveLoop()
        {
            string path = Path.Combine(Paths.Plugins, "Diagnostics");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Log.Debug($"{path} Created", PluginHandler.Instance.Config.VerbouseOutput);
            }
            else
            {
                Log.Debug($"{path} Exists", PluginHandler.Instance.Config.VerbouseOutput);
            }

            path = Path.Combine(path, Server.Port.ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Log.Debug($"{path} Created", PluginHandler.Instance.Config.VerbouseOutput);
            }
            else
            {
                Log.Debug($"{path} Exists", PluginHandler.Instance.Config.VerbouseOutput);
            }

            DateTime now = DateTime.Now;

            string lastDay = now.ToString("yyyy-MM-dd");
            string day;
            string internalPath;

            Log.Debug($"Starting Loop", PluginHandler.Instance.Config.VerbouseOutput);
            while (true)
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
                        Log.Debug($"Created {internalPath}", PluginHandler.Instance.Config.VerbouseOutput);
                    }

                    // Log.Debug($"{Paths.Configs}/{Server.Port}/{day}/{DateTime.Now.ToString("yyyy-MM-dd_HH")}.log");
                    string filePath = Path.Combine(internalPath, $"{now:yyyy-MM-dd_HH}.log");
                    if (!File.Exists(filePath))
                    {
                        if (now.Hour == 0)
                            Analizer.AnalizeContent(Path.Combine(path, lastDay, $"{now.AddDays(-1):yyyy-MM-dd}_23.log"));
                        else
                            Analizer.AnalizeContent(Path.Combine(internalPath, $"{now.AddHours(-1):yyyy-MM-dd_HH}.log"));
                    }

                    if (lastDay != day)
                    {
                        Compress(Path.Combine(path, lastDay));
                        lastDay = day;
                    }

                    lock (Backlog)
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

                    lock (ErrorBacklog)
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
