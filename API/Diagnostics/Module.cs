// -----------------------------------------------------------------------
// <copyright file="Module.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Interfaces;
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
        /// <returns>Courotine handle returned by called function.</returns>
        public static MEC.CoroutineHandle CallSafeDelayed(float delay, Action action, string name)
        {
            return MEC.Timing.CallDelayed(delay, () =>
            {
                try
                {
                    action();
                }
                catch (System.Exception ex)
                {
                    MasterHandler.LogError(ex, null, name);
                    Exiled.API.Features.Log.Error($"[Rouge: {name}] {ex.Message}");
                    Exiled.API.Features.Log.Error($"[Rouge: {name}] {ex.StackTrace}");
                }
            });
        }

        /// <summary>
        /// Calls <see cref="MEC.Timing.RunCoroutine(IEnumerator{float})"/> and reroutes exceptions.
        /// </summary>
        /// <param name="courotine">Delay passed to called function.</param>
        /// <param name="name">Courotine name.</param>
        /// <returns>Courotine handle returned by called function.</returns>
        public static MEC.CoroutineHandle RunSafeCoroutine(IEnumerator<float> courotine, string name)
        {
            courotine.RerouteExceptions((ex) =>
            {
                MasterHandler.LogError(ex, null, name);
                Exiled.API.Features.Log.Error($"[Rouge: {name}] {ex.Message}");
                Exiled.API.Features.Log.Error($"[Rouge: {name}] {ex.StackTrace}");
            });
            return MEC.Timing.RunCoroutine(courotine);
        }

        /// <summary>
        /// Enables all modules that has <see cref="Module.Enabled"/> set to <see langword="true"/> from specific plugin.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void OnEnable(IPlugin<IConfig> plugin)
        {
            foreach (var item in Modules[plugin].Where(i => i.Enabled))
            {
                MasterHandler.Ini();
                Exiled.API.Features.Log.Debug($"Enabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                try
                {
                    item.OnEnable();
                }
                catch (System.Exception ex)
                {
                    MasterHandler.LogError(ex, item, "ENABLING");
                }

                Exiled.API.Features.Log.Debug($"Enabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
            }
        }

        /// <summary>
        /// Disables all modules that has <see cref="Module.Enabled"/> set to <see langword="true"/> from specific plugin.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void OnDisable(IPlugin<IConfig> plugin)
        {
            foreach (var item in Modules[plugin].Where(i => i.Enabled))
            {
                MasterHandler.Ini();
                Exiled.API.Features.Log.Debug($"Disabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                try
                {
                    item.OnDisable();
                }
                catch (System.Exception ex)
                {
                    MasterHandler.LogError(ex, item, "DISABLING");
                }

                Exiled.API.Features.Log.Debug($"Disabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
            }
        }

        /// <summary>
        /// Enables all modules that has <see cref="Module.Enabled"/> set to <see langword="true"/> and <see cref="Module.IsBasic"/> set to <see langword="false"/> except from <paramref name="plugin"/>.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void EnableAllExcept(IPlugin<IConfig> plugin)
        {
            foreach (var module in Modules.Where(p => p.Key != plugin))
            {
                foreach (var item in module.Value.Where(i => i.Enabled && !i.IsBasic))
                {
                    MasterHandler.Ini();
                    Exiled.API.Features.Log.Debug($"Enabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                    try
                    {
                        item.OnEnable();
                    }
                    catch (System.Exception ex)
                    {
                        MasterHandler.LogError(ex, item, "ENABLING");
                    }

                    Exiled.API.Features.Log.Debug($"Enabled {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                }
            }
        }

        /// <summary>
        /// Disables all modules that has <see cref="Module.Enabled"/> set to <see langword="true"/> and <see cref="Module.IsBasic"/> set to <see langword="false"/> except from <paramref name="plugin"/>.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void DisableAllExcept(IPlugin<IConfig> plugin)
        {
            foreach (var module in Modules.Where(p => p.Key != plugin))
            {
                foreach (var item in module.Value.Where(i => i.Enabled && !i.IsBasic))
                {
                    MasterHandler.Ini();
                    Exiled.API.Features.Log.Debug($"Disabling {item.Name} from {plugin.Author}.{plugin.Name}", PluginHandler.Instance.Config.VerbouseOutput);
                    try
                    {
                        item.OnDisable();
                    }
                    catch (System.Exception ex)
                    {
                        MasterHandler.LogError(ex, item, "DISABLING");
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
            this.Log = new ModuleLogger(this.Name);
            this.Plugin = plugin;
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
        /// <returns>Courotine handle returned by called function.</returns>
        public MEC.CoroutineHandle CallDelayed(float delay, Action action, string name = "CallDelayed")
        {
            return MEC.Timing.CallDelayed(delay, () =>
            {
                try
                {
                    action();
                }
                catch (System.Exception ex)
                {
                    MasterHandler.LogError(ex, this, name);
                    this.Log.Error($"[{this.Name}: {name}] {ex.Message}");
                    this.Log.Error($"[{this.Name}: {name}] {ex.StackTrace}");
                }
            });
        }

        /// <summary>
        /// Calls <see cref="MEC.Timing.RunCoroutine(IEnumerator{float})"/> and reroutes exceptions.
        /// </summary>
        /// <param name="courotine">Delay passed to called function.</param>
        /// <param name="name">Courotine name.</param>
        /// <returns>Courotine handle returned by called function.</returns>
        public MEC.CoroutineHandle RunCoroutine(IEnumerator<float> courotine, string name = "RunCoroutine")
        {
            courotine.RerouteExceptions((ex) =>
            {
                MasterHandler.LogError(ex, this, name);
                this.Log.Error($"[{this.Name}: {name}] {ex.Message}");
                this.Log.Error($"[{this.Name}: {name}] {ex.StackTrace}");
            });
            return MEC.Timing.RunCoroutine(courotine);
        }

        internal static readonly Dictionary<IPlugin<IConfig>, List<Module>> Modules = new Dictionary<IPlugin<IConfig>, List<Module>>();

        /// <summary>
        /// Gets plugin that this module belong to.
        /// </summary>
        [JsonIgnore]
        protected IPlugin<IConfig> Plugin { get; }

        /// <summary>
        /// Gets used to use special logging method.
        /// </summary>
        protected ModuleLogger Log { get; }
    }
}
