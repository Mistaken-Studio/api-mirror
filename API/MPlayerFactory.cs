// -----------------------------------------------------------------------
// <copyright file="MPlayer.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using PluginAPI.Core.Factories;
using PluginAPI.Core.Interfaces;

namespace Mistaken.API
{
    internal sealed class MPlayerFactory : PlayerFactory
    {
        public override Type BaseType => typeof(MPlayer);

        public override IPlayer Create(IGameComponent component) => new MPlayer(component);
    }
}