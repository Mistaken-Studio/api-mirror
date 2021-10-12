// -----------------------------------------------------------------------
// <copyright file="CustomSlotsHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using LiteNetLib.Utils;
using Mistaken.API.Diagnostics;

namespace Mistaken.API.CustomSlots
{
    /// <inheritdoc/>
    public class CustomSlotsHandler : Module
    {
        /// <summary>
        /// Players with dynamic reserved slots.
        /// </summary>
        public static readonly HashSet<string> DynamicReservedSlots = new HashSet<string>();

        /// <summary>
        /// Gets real number of slots.
        /// </summary>
        public static int RealSlots => CustomNetworkManager.slots + ConnectedDynamicSlots.Count;

        /// <inheritdoc/>
        public override bool IsBasic => true;

        /// <inheritdoc/>
        public override bool Enabled => PluginHandler.Instance.Config.CustomSlotsEnabled;

        /// <inheritdoc/>
        public override string Name => "CustomSlots";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.PreAuthenticating += this.Handle<Exiled.Events.EventArgs.PreAuthenticatingEventArgs>((ev) => this.Player_PreAuthenticating(ev));
            Exiled.Events.Handlers.Player.Left += this.Handle<Exiled.Events.EventArgs.LeftEventArgs>((ev) => this.Player_Left(ev));
            Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => this.Server_RestartingRound(), "RoundRestart");

            PluginHandler.Instance.Harmony.Patch(typeof(ReservedSlot).GetMethod(nameof(ReservedSlot.HasReservedSlot)), new HarmonyLib.HarmonyMethod(typeof(ReservedSlotPatch).GetMethod(nameof(ReservedSlotPatch.Prefix))));
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.PreAuthenticating -= this.Handle<Exiled.Events.EventArgs.PreAuthenticatingEventArgs>((ev) => this.Player_PreAuthenticating(ev));
            Exiled.Events.Handlers.Player.Left -= this.Handle<Exiled.Events.EventArgs.LeftEventArgs>((ev) => this.Player_Left(ev));
            Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => this.Server_RestartingRound(), "RoundRestart");

            PluginHandler.Instance.Harmony.Unpatch(typeof(ReservedSlot).GetMethod(nameof(ReservedSlot.HasReservedSlot)), typeof(ReservedSlotPatch).GetMethod(nameof(ReservedSlotPatch.Prefix)));
        }

        internal CustomSlotsHandler(PluginHandler plugin)
            : base(plugin)
        {
        }

        private static readonly HashSet<string> ConnectedDynamicSlots = new HashSet<string>();
        private static readonly HashSet<string> ConnectedReservedSlots = new HashSet<string>();

        private void Server_RestartingRound()
        {
            ConnectedDynamicSlots.Clear();
            ConnectedReservedSlots.Clear();
        }

        private void Player_Left(Exiled.Events.EventArgs.LeftEventArgs ev)
        {
            ConnectedDynamicSlots.Remove(ev.Player.UserId);
            ConnectedReservedSlots.Remove(ev.Player.UserId);
        }

        private void Player_PreAuthenticating(Exiled.Events.EventArgs.PreAuthenticatingEventArgs ev)
        {
            this.Log.Debug("Connecting player", PluginHandler.Instance.Config.VerbouseOutput);

            if (ev.ServerFull)
            {
                this.Log.Warn("Server is full, can't grant reserved slots");
                return;
            }

            if (ReservedSlot.Users.Contains(ev.UserId))
                this.Log.Debug("Connecting player with static reserved slot", PluginHandler.Instance.Config.VerbouseOutput);
            if (DynamicReservedSlots.Contains(ev.UserId))
                this.Log.Debug("Connecting player with dynamic reserved slot", PluginHandler.Instance.Config.VerbouseOutput);

            // Override
            if (((CentralAuthPreauthFlags)ev.Flags).HasFlagFast(CentralAuthPreauthFlags.ReservedSlot))
            {
                this.Log.Debug("Player has active ReservedSlot flag", PluginHandler.Instance.Config.VerbouseOutput);
                ConnectedReservedSlots.Add(ev.UserId);
                return;
            }

            // Server Not full
            if (Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.ConnectedPeersCount < RealSlots)
                return;

            // Has Dynamic Reserved Slot
            else if (DynamicReservedSlots.Contains(ev.UserId))
                ConnectedDynamicSlots.Add(ev.UserId);

            // Has Reserved Slot
            else if (ReservedSlot.Users.Contains(ev.UserId) || DynamicReservedSlots.Contains(ev.UserId))
            {
                if (Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.ConnectedPeersCount >= RealSlots + CustomNetworkManager.reservedSlots)
                {
                    string reason = $"Server is full!   {Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.ConnectedPeersCount}/{RealSlots + CustomNetworkManager.reservedSlots} (With reserved slots)";
                    var writer = new NetDataWriter();
                    writer.Put((byte)10);
                    writer.Put(reason);
                    ev.Request.Reject(writer);
                    ev.Disallow();
                    Exiled.API.Features.Log.Info($"Rejecting {ev.UserId} with reason: {reason}");
                }
                else
                    ConnectedReservedSlots.Add(ev.UserId);
            }

            // No reserved Slot
            else
            {
                string reason = $"Server is full!   {Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.ConnectedPeersCount}/{RealSlots}";
                var writer = new NetDataWriter();
                writer.Put((byte)10);
                writer.Put(reason);
                ev.Request.Reject(writer);
                ev.Disallow();
                Exiled.API.Features.Log.Info($"Rejecting {ev.UserId} with reason: {reason}");
            }
        }
    }
}
