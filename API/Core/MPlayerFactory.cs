using PluginAPI.Core;
using PluginAPI.Core.Factories;
using PluginAPI.Core.Interfaces;
using System;

namespace Mistaken.API.Core;

public sealed class MPlayerFactory : PlayerFactory
{
    public override Type BaseType => typeof(Player);

    public override IPlayer Create(IGameComponent component) => new MPlayer(component);
}