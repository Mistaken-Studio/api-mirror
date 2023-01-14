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