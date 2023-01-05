// -----------------------------------------------------------------------
// <copyright file="Module.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Calls <see cref="Timing.CallDelayed(float, Action)"/> and adds try catch to action.
        /// </summary>
        /// <param name="delay">Delay passed to called function.</param>
        /// <param name="action">Action passed to called function.</param>
        /// <param name="name">Function name.</param>
        /// <param name="terminateAfterRoundRestart">If job coroutine should be killed when round restarts.</param>
        /// <returns>Coroutine handle returned by called function.</returns>
        public static CoroutineHandle CallSafeDelayed(float delay, Action action, string name, bool terminateAfterRoundRestart)
        {
            var tor = Timing.CallDelayed(delay, () =>
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
        public static CoroutineHandle CallSafeDelayed(float delay, Action action, string name)
            => CallSafeDelayed(delay, action, name, false);

        /// <summary>
        /// Calls <see cref="MEC.Timing.RunCoroutine(IEnumerator{float})"/> and reroutes exceptions.
        /// </summary>
        /// <param name="courotine">Delay passed to called function.</param>
        /// <param name="name">Coroutine name.</param>
        /// <param name="terminateAfterRoundRestart">If job coroutine should be killed when round restarts.</param>
        /// <returns>Coroutine handle returned by called function.</returns>
        public static CoroutineHandle RunSafeCoroutine(IEnumerator<float> courotine, string name, bool terminateAfterRoundRestart)
        {
            courotine = courotine.RerouteExceptions((ex) =>
            {
                MasterHandler.LogError(ex, null, name);
                Exiled.API.Features.Log.Error($"[Rouge: {name}] {ex}");
            });
            var tor = Timing.RunCoroutine(courotine);
            if (terminateAfterRoundRestart)
                ToTerminateAfterRoundRestart.Add(tor);
            return tor;
        }

        /// <inheritdoc cref="RunSafeCoroutine(IEnumerator{float}, string, bool)"/>
        public static CoroutineHandle RunSafeCoroutine(IEnumerator<float> courotine, string name)
            => RunSafeCoroutine(courotine, name, false);

        /// <summary>
        /// Creates coroutine that calls <paramref name="innerLoop"/> through all of round.
        /// </summary>
        /// <param name="innerLoop">Coroutine executed in round. Has to contain own delay!.</param>
        /// <param name="name">Coroutine name.</param>
        /// <returns>Coroutine handle returned by called function.</returns>
        public static CoroutineHandle CreateSafeRoundLoop(Func<IEnumerator<float>> innerLoop, string name)
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
                Exiled.API.Features.Log.Debug($"Enabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Debug);
                try
                {
                    item.OnEnable();
                }
                catch (Exception ex)
                {
                    MasterHandler.LogError(ex, item, "OnEnable");
                }

                Exiled.API.Features.Log.Debug($"Enabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Debug);
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
                Exiled.API.Features.Log.Debug($"Disabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Debug);
                try
                {
                    item.OnDisable();
                    Timing.KillCoroutines(item.coroutines.ToArray());
                }
                catch (Exception ex)
                {
                    MasterHandler.LogError(ex, item, "OnDisable");
                }

                Exiled.API.Features.Log.Debug($"Disabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Debug);
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
                    Exiled.API.Features.Log.Debug($"Enabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Debug);
                    try
                    {
                        item.OnEnable();
                    }
                    catch (Exception ex)
                    {
                        MasterHandler.LogError(ex, item, "OnEnable");
                    }

                    Exiled.API.Features.Log.Debug($"Enabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Debug);
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
                    Exiled.API.Features.Log.Debug($"Disabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Debug);
                    try
                    {
                        item.OnDisable();
                        Timing.KillCoroutines(item.coroutines.ToArray());
                    }
                    catch (Exception ex)
                    {
                        MasterHandler.LogError(ex, item, "OnDisable");
                    }

                    Exiled.API.Features.Log.Debug($"Disabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Debug);
                }
            }
        }

        /// <summary>
        /// Registers handler by creating it's instance.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        /// <typeparam name="T">Type of handler.</typeparam>
        public static void RegisterHandler<T>(IPlugin<IConfig> plugin)
            where T : Module
        {
            foreach (var ctor in typeof(T).GetConstructors(
                         BindingFlags.Public |
                         BindingFlags.NonPublic |
                         BindingFlags.Instance))
            {
                if (ctor.DeclaringType?.IsAbstract ?? true)
                    continue;

                if (!typeof(IPlugin<IConfig>)
                        .IsAssignableFrom(ctor.GetParameters().SingleOrDefault()?.ParameterType))
                    continue;

                ctor.Invoke(new object[] { plugin });
                return;
            }

            throw new ArgumentException(
                $"Missing constructor with {nameof(IPlugin<IConfig>)} as only param",
                nameof(plugin));
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
        /// Gets a value indicating whether if is required for basic functions.
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
        /// Calls <see cref="Timing.CallDelayed(float, Action)"/> and adds try catch to action.
        /// </summary>
        /// <param name="delay">Delay passed to called function.</param>
        /// <param name="action">Action passed to called function.</param>
        /// <param name="name">Function name.</param>
        /// <param name="terminateAfterRoundRestart">If job coroutine should be killed when round restarts.</param>
        /// <returns>Coroutine handle returned by called function.</returns>
        public CoroutineHandle CallDelayed(float delay, Action action, string name, bool terminateAfterRoundRestart)
        {
            var tor = Timing.CallDelayed(delay, () =>
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

            this.coroutines.Add(tor);
            if (terminateAfterRoundRestart)
                ToTerminateAfterRoundRestart.Add(tor);

            return tor;
        }

        /// <inheritdoc cref="CallDelayed(float, Action, string, bool)"/>
        public CoroutineHandle CallDelayed(float delay, Action action, string name = "CallDelayed")
            => this.CallDelayed(delay, action, name, false);

        /// <summary>
        /// Calls <see cref="Timing.RunCoroutine(IEnumerator{float})"/> and reroutes exceptions.
        /// </summary>
        /// <param name="courotine">Delay passed to called function.</param>
        /// <param name="name">Coroutine name.</param>
        /// <param name="terminateAfterRoundRestart">If job coroutine should be killed when round restarts.</param>
        /// <returns>Coroutine handle returned by called function.</returns>
        public CoroutineHandle RunCoroutine(IEnumerator<float> courotine, string name, bool terminateAfterRoundRestart)
        {
            courotine = courotine.RerouteExceptions((ex) =>
            {
                MasterHandler.LogError(ex, this, name);
                this.Log.Error($"[{this.Name}: {name}] {ex}");
            });
            var tor = Timing.RunCoroutine(courotine);
            this.coroutines.Add(tor);
            if (terminateAfterRoundRestart)
                ToTerminateAfterRoundRestart.Add(tor);
            return tor;
        }

        /// <inheritdoc cref="RunCoroutine(IEnumerator{float}, string, bool)"/>
        public CoroutineHandle RunCoroutine(IEnumerator<float> courotine, string name = "RunCoroutine")
            => this.RunCoroutine(courotine, name, false);

        /// <summary>
        /// Creates coroutine that calls <paramref name="innerLoop"/> through all of round.
        /// </summary>
        /// <param name="innerLoop">Coroutine executed in round. Has to contain own delay!.</param>
        /// <param name="name">Coroutine name.</param>
        /// <returns>Coroutine handle returned by called function.</returns>
        public CoroutineHandle CreateRoundLoop(Func<IEnumerator<float>> innerLoop, string name = "RoundLoop")
        {
            var tor = this.RunCoroutine(RoundLoop(innerLoop), name, true);
            this.coroutines.Add(tor);
            return tor;
        }

        internal static readonly Dictionary<IPlugin<IConfig>, List<Module>> Modules = new();

        internal static void TerminateAllCoroutines()
        {
            Timing.KillCoroutines(ToTerminateAfterRoundRestart.ToArray());
            ToTerminateAfterRoundRestart.Clear();
        }

        /// <summary>
        /// Gets plugin that this module belong to.
        /// </summary>
        [JsonIgnore]
        protected internal IPlugin<IConfig> Plugin { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// Default Constructor.
        /// </summary>
        /// <param name="plugin">Plugin creating module.</param>
        protected Module(IPlugin<IConfig> plugin)
        {
            this.Plugin = plugin;
            this.Log = new(this);
            if (!Modules.ContainsKey(plugin))
            {
                MasterHandler.CurrentStatus.LoadedPlugins++;
                Modules.Add(plugin, new());
            }

            Modules[plugin].RemoveAll(i => i.Name == this.Name);
            Modules[plugin].Add(this);
            MasterHandler.CurrentStatus.LoadedModules++;
        }

        /// <summary>
        /// Gets used to use special logging method.
        /// </summary>
        [JsonIgnore]
        protected ModuleLogger Log { get; }

        private static readonly List<CoroutineHandle> ToTerminateAfterRoundRestart = new();

        private static IEnumerator<float> RoundLoop(Func<IEnumerator<float>> innerLoop)
        {
            yield return Timing.WaitUntilTrue(() => Exiled.API.Features.Round.IsStarted);

            while (Exiled.API.Features.Round.IsStarted)
                yield return innerLoop().WaitUntilDone();
        }

        private readonly List<CoroutineHandle> coroutines = new();
    }
}
