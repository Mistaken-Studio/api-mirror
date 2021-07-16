﻿// -----------------------------------------------------------------------
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
        public ModuleLogger(string module) => this.module = module;

        /// <inheritdoc cref="Log.Debug(object, bool)"/>
        public void Debug(object message, bool canBeSant/* = true*/) => Log.Debug($"[{this.module}] {message}", canBeSant);

        /// <inheritdoc cref="Log.Info(object)"/>
        public void Info(object message) => Log.Info($"[{this.module}] {message}");

        /// <inheritdoc cref="Log.Warn(object)"/>
        public void Warn(object message) => Log.Warn($"[{this.module}] {message}");

        /// <inheritdoc cref="Log.Error(object)"/>
        public void Error(object message) => Log.Error($"[{this.module}] {message}");

        private readonly string module;
    }
}
