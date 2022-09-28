// -----------------------------------------------------------------------
// <copyright file="SynchronizerControllerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Mistaken.API.Toys.Components.Synchronizers;
using UnityEngine;

#pragma warning disable SA1401 // Fields should be private

// ReSharper disable UnusedMember.Local
// ReSharper disable once IdentifierTypo
namespace Mistaken.API.Toys.Components.Controllers
{
    internal class SynchronizerControllerScript : MonoBehaviour
    {
        public void AddSubscriber(Player player)
        {
            if (this.subscribers.Contains(player))
                return;

            this.subscribers.Add(player);

            foreach (var synchronizerScript in this.SynchronizerScripts.Where(x => x is PrimitiveSynchronizerScript).ToArray())
            {
                try
                {
                    synchronizerScript.ShowFor(player);
                    synchronizerScript.UpdateSubscriber(player);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }
            }

            foreach (var synchronizerScript in this.SynchronizerScripts.Where(x => !(x is PrimitiveSynchronizerScript)))
            {
                synchronizerScript.UpdateSubscriber(player);
            }
        }

        public void RemoveSubscriber(Player player, bool forceHideObjects = false)
        {
            this.subscribers.Remove(player);

            foreach (var synchronizerScript in this.SynchronizerScripts.Where(x => x is PrimitiveSynchronizerScript))
                synchronizerScript.HideFor(player, forceHideObjects);
        }

        public bool IsSubscriber(Player player)
            => this.subscribers.Contains(player);

        public virtual IEnumerable<Player> GetSubscribers()
            => this.subscribers;

        public void SyncFor(Player player)
            => this.SynchronizerScripts.ForEach(x => x.UpdateSubscriber(player));

        internal virtual void AddScript(SynchronizerScript script)
        {
            this.SynchronizerScripts.Add(script);
            script.Controller = this;
        }

        internal virtual void RemoveScript(SynchronizerScript script)
        {
            this.SynchronizerScripts.Remove(script);
        }

        protected readonly List<SynchronizerScript> SynchronizerScripts = new List<SynchronizerScript>();

        private readonly HashSet<Player> subscribers = new HashSet<Player>();
    }
}
