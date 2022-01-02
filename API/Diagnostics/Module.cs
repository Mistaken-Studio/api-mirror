// -----------------------------------------------------------------------
// <copyright file="Module.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Interfaces;
using MEC;
using Newtonsoft.Json;

namespace Mistaken.API.Diagnostics
{
    /// <summary>
    /// Diagnostics module.
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// Calls <see cref="MEC.Timing.CallDelayed(float, Action)"/> and adds try catch to action.
        /// </summary>
        /// <param name="delay">Delay passed to called function.</param>
        /// <param name="action">Action passed to called function.</param>
        /// <param name="name">Function name.</param>
        /// <param name="terminateAfterRoundRestart">If job courutine should be killed when round restarts.</param>
        /// <returns>Courotine handle returned by called function.</returns>
        public static MEC.CoroutineHandle CallSafeDelayed(float delay, Action action, string name, bool terminateAfterRoundRestart)
        {
            var tor = MEC.Timing.CallDelayed(delay, () =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    MasterHandler.LogError(ex, null, name);
                    Exiled.API.Features.Log.Error($"[Rouge: {name}] {ex}");
                }
            });

            if (terminateAfterRoundRestart)
                ToTerminateAfterRoundRestart.Add(tor);

            return tor;
        }

        /// <inheritdoc cref="CallSafeDelayed(float, Action, string, bool)"/>
        public static MEC.CoroutineHandle CallSafeDelayed(float delay, Action action, string name)
            => CallSafeDelayed(delay, action, name, false);

        /// <summary>
        /// Calls <see cref="MEC.Timing.RunCoroutine(IEnumerator{float})"/> and reroutes exceptions.
        /// </summary>
        /// <param name="courotine">Delay passed to called function.</param>
        /// <param name="name">Courotine name.</param>
        /// <param name="terminateAfterRoundRestart">If job courutine should be killed when round restarts.</param>
        /// <returns>Courotine handle returned by called function.</returns>
        public static MEC.CoroutineHandle RunSafeCoroutine(IEnumerator<float> courotine, string name, bool terminateAfterRoundRestart)
        {
            courotine = courotine.RerouteExceptions((ex) =>
            {
                MasterHandler.LogError(ex, null, name);
                Exiled.API.Features.Log.Error($"[Rouge: {name}] {ex}");
            });
            var tor = MEC.Timing.RunCoroutine(courotine);
            if (terminateAfterRoundRestart)
                ToTerminateAfterRoundRestart.Add(tor);
            return tor;
        }

        /// <inheritdoc cref="RunSafeCoroutine(IEnumerator{float}, string, bool)"/>
        public static MEC.CoroutineHandle RunSafeCoroutine(IEnumerator<float> courotine, string name)
            => RunSafeCoroutine(courotine, name, false);

        /// <summary>
        /// Creates courotine that calls <paramref name="innerLoop"/> through all of round.
        /// </summary>
        /// <param name="innerLoop">Courotine executed in round. Has to contain own delay!.</param>
        /// <param name="name">Courotine name.</param>
        /// <returns>Courotine handle returned by called function.</returns>
        public static MEC.CoroutineHandle CreateSafeRoundLoop(IEnumerator<float> innerLoop, string name)
        {
            return RunSafeCoroutine(RoundLoop(innerLoop), name, true);
        }

        /// <summary>
        /// Enables all modules that has <see cref="Enabled"/> set to <see langword="true"/> from specific plugin.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void OnEnable(IPlugin<IConfig> plugin)
        {
            MasterHandler.Ini();

            foreach (var item in Modules[plugin].Where(i => i.Enabled))
            {
                Exiled.API.Features.Log.Debug($"Enabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                try
                {
                    item.OnEnable();
                }
                catch (Exception ex)
                {
                    MasterHandler.LogError(ex, item, "OnEnable");
                }

                Exiled.API.Features.Log.Debug($"Enabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
            }
        }

        /// <summary>
        /// Disables all modules that has <see cref="Enabled"/> set to <see langword="true"/> from specific plugin.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void OnDisable(IPlugin<IConfig> plugin)
        {
            MasterHandler.Ini();

            foreach (var item in Modules[plugin].Where(i => i.Enabled))
            {
                Exiled.API.Features.Log.Debug($"Disabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                try
                {
                    item.OnDisable();
                }
                catch (Exception ex)
                {
                    MasterHandler.LogError(ex, item, "OnDisable");
                }

                Exiled.API.Features.Log.Debug($"Disabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
            }
        }

        /// <summary>
        /// Enables all modules that has <see cref="Enabled"/> set to <see langword="true"/> and <see cref="IsBasic"/> set to <see langword="false"/> except from <paramref name="plugin"/>.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void EnableAllExcept(IPlugin<IConfig> plugin)
        {
            MasterHandler.Ini();

            foreach (var module in Modules.Where(p => p.Key != plugin))
            {
                foreach (var item in module.Value.Where(i => i.Enabled && !i.IsBasic))
                {
                    Exiled.API.Features.Log.Debug($"Enabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                    try
                    {
                        item.OnEnable();
                    }
                    catch (Exception ex)
                    {
                        MasterHandler.LogError(ex, item, "OnEnable");
                    }

                    Exiled.API.Features.Log.Debug($"Enabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                }
            }
        }

        /// <summary>
        /// Disables all modules that has <see cref="Enabled"/> set to <see langword="true"/> and <see cref="IsBasic"/> set to <see langword="false"/> except from <paramref name="plugin"/>.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void DisableAllExcept(IPlugin<IConfig> plugin)
        {
            MasterHandler.Ini();

            foreach (var module in Modules.Where(p => p.Key != plugin))
            {
                foreach (var item in module.Value.Where(i => i.Enabled && !i.IsBasic))
                {
                    Exiled.API.Features.Log.Debug($"Disabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                    try
                    {
                        item.OnDisable();
                    }
                    catch (Exception ex)
                    {
                        MasterHandler.LogError(ex, item, "OnDisable");
                    }

                    Exiled.API.Features.Log.Debug($"Disabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// Default Constructor.
        /// </summary>
        /// <param name="plugin">Plugin creating module.</param>
        public Module(IPlugin<IConfig> plugin)
        {
            this.Plugin = plugin;
            this.Log = new ModuleLogger(this);
            if (!Modules.ContainsKey(plugin))
            {
                MasterHandler.CurrentStatus.LoadedPlugins++;
                Modules.Add(plugin, new List<Module>());
            }

            Modules[plugin].RemoveAll(i => i.Name == this.Name);
            Modules[plugin].Add(this);
            MasterHandler.CurrentStatus.LoadedModules++;
        }

        /// <summary>
        /// Gets module Name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether if module should be enabled.
        /// </summary>
        public virtual bool Enabled { get; protected set; } = true;

        /// <summary>
        /// Gets a value indicating whether if is requied for basic functions.
        /// </summary>
        public virtual bool IsBasic { get; } = false;

        /// <summary>
        /// Called when enabling.
        /// </summary>
        public abstract void OnEnable();

        /// <summary>
        /// Called when disabling.
        /// </summary>
        public abstract void OnDisable();

        /// <summary>
        /// Calls <see cref="MEC.Timing.CallDelayed(float, Action)"/> and adds try catch to action.
        /// </summary>
        /// <param name="delay">Delay passed to called function.</param>
        /// <param name="action">Action passed to called function.</param>
        /// <param name="name">Function name.</param>
        /// <param name="terminateAfterRoundRestart">If job courutine should be killed when round restarts.</param>
        /// <returns>Courotine handle returned by called function.</returns>
        public MEC.CoroutineHandle CallDelayed(float delay, Action action, string name, bool terminateAfterRoundRestart)
        {
            var tor = MEC.Timing.CallDelayed(delay, () =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    MasterHandler.LogError(ex, this, name);
                    this.Log.Error($"[{this.Name}: {name}] {ex}");
                }
            });

            if (terminateAfterRoundRestart)
                ToTerminateAfterRoundRestart.Add(tor);

            return tor;
        }

        /// <inheritdoc cref="CallDelayed(float, Action, string, bool)"/>
        public MEC.CoroutineHandle CallDelayed(float delay, Action action, string name = "CallDelayed")
            => this.CallDelayed(delay, action, name, false);

        /// <summary>
        /// Calls <see cref="MEC.Timing.RunCoroutine(IEnumerator{float})"/> and reroutes exceptions.
        /// </summary>
        /// <param name="courotine">Delay passed to called function.</param>
        /// <param name="name">Courotine name.</param>
        /// <param name="terminateAfterRoundRestart">If job courutine should be killed when round restarts.</param>
        /// <returns>Courotine handle returned by called function.</returns>
        public MEC.CoroutineHandle RunCoroutine(IEnumerator<float> courotine, string name, bool terminateAfterRoundRestart)
        {
            courotine = courotine.RerouteExceptions((ex) =>
            {
                MasterHandler.LogError(ex, this, name);
                this.Log.Error($"[{this.Name}: {name}] {ex}");
            });
            var tor = MEC.Timing.RunCoroutine(courotine);
            if (terminateAfterRoundRestart)
                ToTerminateAfterRoundRestart.Add(tor);
            return tor;
        }

        /// <inheritdoc cref="RunCoroutine(IEnumerator{float}, string, bool)"/>
        public MEC.CoroutineHandle RunCoroutine(IEnumerator<float> courotine, string name = "RunCoroutine")
            => this.RunCoroutine(courotine, name, false);

        /// <summary>
        /// Creates courotine that calls <paramref name="innerLoop"/> through all of round.
        /// </summary>
        /// <param name="innerLoop">Courotine executed in round. Has to contain own delay!.</param>
        /// <param name="name">Courotine name.</param>
        /// <returns>Courotine handle returned by called function.</returns>
        public MEC.CoroutineHandle CreateRoundLoop(IEnumerator<float> innerLoop, string name = "RoundLoop")
        {
            return this.RunCoroutine(RoundLoop(innerLoop), name, true);
        }

        internal static readonly Dictionary<IPlugin<IConfig>, List<Module>> Modules = new Dictionary<IPlugin<IConfig>, List<Module>>();

        internal static void Server_WaitingForPlayer()
        {
            MEC.Timing.KillCoroutines(ToTerminateAfterRoundRestart.ToArray());
            ToTerminateAfterRoundRestart.Clear();
        }

        /// <summary>
        /// Gets plugin that this module belong to.
        /// </summary>
        [JsonIgnore]
        protected internal IPlugin<IConfig> Plugin { get; }

        /// <summary>
        /// Gets used to use special logging method.
        /// </summary>
        [JsonIgnore]
        protected ModuleLogger Log { get; }

        private static readonly List<MEC.CoroutineHandle> ToTerminateAfterRoundRestart = new List<MEC.CoroutineHandle>();

        private static IEnumerator<float> RoundLoop(IEnumerator<float> innerLoop)
        {
            yield return Timing.WaitUntilTrue(() => Exiled.API.Features.Round.IsStarted);

            while (Exiled.API.Features.Round.IsStarted)
            {
                innerLoop.Reset();
                yield return innerLoop.WaitUntilDone();
            }
        }
    }
}
