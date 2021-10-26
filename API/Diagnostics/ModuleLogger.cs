// -----------------------------------------------------------------------
// <copyright file="ModuleLogger.cs" company="Mistaken">
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
    /// Used to Log with prefix.
    /// </summary>
    public class ModuleLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleLogger"/> class.
        /// Constructor.
        /// </summary>
        public ModuleLogger(Module module)
            => this.module = module;

        /// <inheritdoc cref="Log.Debug(object, bool)"/>
        public void Debug(object message, bool canBeSent/* = true*/)
        {
            if (canBeSent)
                Log.Send($"[{this.module.Plugin}.{this.module.Name}] {message}", Discord.LogLevel.Debug, ConsoleColor.Green);
        }

        /// <inheritdoc cref="Log.Info(object)"/>
        public void Info(object message)
            => Log.Send($"[{this.module.Plugin}.{this.module.Name}] {message}", Discord.LogLevel.Info, ConsoleColor.Cyan);

        /// <inheritdoc cref="Log.Warn(object)"/>
        public void Warn(object message)
            => Log.Send($"[{this.module.Plugin}.{this.module.Name}] {message}", Discord.LogLevel.Warn, ConsoleColor.DarkYellow);

        /// <inheritdoc cref="Log.Error(object)"/>
        public void Error(object message)
            => Log.Send($"[{this.module.Plugin}.{this.module.Name}] {message}", Discord.LogLevel.Error, ConsoleColor.DarkRed);

        private readonly Module module;
    }
}
