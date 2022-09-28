// -----------------------------------------------------------------------
// <copyright file="GlobalSynchronizerControllerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Mirror;

namespace Mistaken.API.Toys
{
    internal class GlobalSynchronizerControllerScript : SynchronizerControllerScript
    {
        public override IEnumerable<Player> GetSubscribers()
            => RealPlayers.List;

        internal override void AddScript(SynchronizerScript script)
        {
            base.AddScript(script);
            NetworkServer.Spawn(script.gameObject);
        }
    }
}