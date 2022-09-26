// -----------------------------------------------------------------------
// <copyright file="SynchronizerControllerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

// ReSharper disable UnusedMember.Local
// ReSharper disable once IdentifierTypo
namespace Mistaken.API.Toys
{
    internal class SynchronizerControllerScript : MonoBehaviour
    {
        public void AddSubscriber(Player player)
        {
            if (this.subscribers.Contains(player))
                return;

            this.subscribers.Add(player);
            foreach (var light in this.synchronizerScripts)
            {
                light.UpdateSubscriber(player);
            }
        }

        public void RemoveSubscriber(Player player)
        {
            this.subscribers.Remove(player);
        }

        public bool IsSubscriber(Player player)
            => this.subscribers.Contains(player);

        public virtual IEnumerable<Player> GetSubscribers()
            => this.subscribers;

        public void SyncFor(Player player)
            => this.synchronizerScripts.ForEach(x => x.UpdateSubscriber(player));

        internal void AddScript(SynchronizerScript script)
        {
            this.synchronizerScripts.Add(script);
            script.Controller = this;
        }

        private readonly HashSet<Player> subscribers = new HashSet<Player>();

        private readonly List<SynchronizerScript> synchronizerScripts = new List<SynchronizerScript>();
    }

    internal class GlobalSynchronizerControllerScript : SynchronizerControllerScript
    {
        public override IEnumerable<Player> GetSubscribers()
            => RealPlayers.List;
    }
}
