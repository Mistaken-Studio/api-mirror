using PluginAPI.Core.Factories;
using PluginAPI.Core.Interfaces;
using System;

namespace Mistaken.API
{
    internal sealed class MPlayerFactory : PlayerFactory
    {
        public override Type BaseType => typeof(MPlayer);

        public override IPlayer Create(IGameComponent component) => new MPlayer(component);
    }
}