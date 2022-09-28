// -----------------------------------------------------------------------
// <copyright file="SynchronizerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using AdminToys;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

#pragma warning disable SA1116 // Split parameters should start on line after declaration
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1401 // Fields should be private

// ReSharper disable UnusedMember.Local
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Mistaken.API.Toys
{
    internal abstract class SynchronizerScript : MonoBehaviour
    {
        static SynchronizerScript()
        {
            SynchronizerScript.MakeCustomSyncWriter = typeof(MirrorExtensions)
                .GetMethod("MakeCustomSyncWriter",
                    BindingFlags.NonPublic | BindingFlags.Static);
        }

        internal SynchronizerControllerScript Controller { get; set; }

        internal AdminToyBase Toy { get; set; }

        internal bool SyncPosition { get; set; } = true;

        internal bool SyncRotation { get; set; } = true;

        internal bool SyncScale { get; set; } = true;

        internal void UpdateSubscriber(Player player)
        {
            var state = this.GetPlayerState(player);

            if (state == this.CurrentState)
                return;

            this.SyncFor(player, state);

            this.CurrentState.CopyValues(state);
        }

        internal void ShowFor(Player player)
        {
            var state = this.GetPlayerState(player);

            if (state.Visible)
                return;

            if (Server.SendSpawnMessage is null)
                throw new NullReferenceException($"{nameof(Server.SendSpawnMessage)} was null");

            if (player.Connection is null)
                throw new NullReferenceException($"{nameof(player.Connection)} was null");

            Server.SendSpawnMessage.Invoke(null, new object[] { this.Toy.netIdentity, player.Connection });

            state.Visible = true;

            this.ResetState(state);
        }

        internal void HideFor(Player player)
        {
            var state = this.GetPlayerState(player);

            if (!state.Visible)
                return;

            if (player.Connection is null)
                throw new NullReferenceException($"{nameof(player.Connection)} was null");

            player.Connection.Send(new ObjectDestroyMessage { netId = this.Toy.netId }, 0);

            state.Visible = false;
        }

        protected static readonly MethodInfo MakeCustomSyncWriter;
        protected readonly Dictionary<Player, State> LastStates = new Dictionary<Player, State>();

        protected Type ToyType => this.Toy.GetType();

        protected abstract State CurrentState { get; }

        protected virtual ulong GetStateFlags(State playerState)
        {
            return (this.SyncPosition && this.CurrentState.Position != playerState.Position ? 1UL : 0UL)
                   + (this.SyncRotation && this.CurrentState.Rotation != playerState.Rotation ? 2UL : 0UL)
                   + (this.SyncScale && this.CurrentState.Scale != playerState.Scale ? 4UL : 0UL)
                   + (playerState.Visible ? (1UL << 63) : (1UL << 62));
        }

        protected virtual Action<NetworkWriter> CustomSyncVarGenerator(ulong flags, Action<NetworkWriter> callBackAction = null)
        {
            return targetWriter =>
            {
                try
                {
                    targetWriter.WriteUInt64(flags & 7); // position (1) + rotation (2) + scale (4)

                    if ((flags & 1) != 0) targetWriter.WriteVector3(this.CurrentState.Position);
                    if ((flags & 2) != 0) targetWriter.WriteLowPrecisionQuaternion(this.CurrentState.Rotation);
                    if ((flags & 4) != 0) targetWriter.WriteVector3(this.CurrentState.Scale);

                    try
                    {
                        callBackAction?.Invoke(targetWriter);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception when calling callbackAction");
                        Log.Error(ex);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception when Writing writer");
                    Log.Error("targetWriter is null: " + (targetWriter is null));
                    Log.Error("CurrentState is null: " + (this.CurrentState is null));
                    Log.Error(ex);
                }
            };
        }

        protected virtual bool ShouldUpdate()
        {
            if ((this.SyncPosition && this.gameObject.transform.position != this.CurrentState.Position) ||
                (this.SyncRotation && this.gameObject.transform.rotation != this.CurrentState.Rotation.Value) ||
                (this.SyncScale && this.gameObject.transform.lossyScale != this.CurrentState.Scale))
            {
                this.CurrentState.Position = this.gameObject.transform.position;
                this.CurrentState.Rotation = new LowPrecisionQuaternion(this.gameObject.transform.rotation);
                this.CurrentState.Scale = this.gameObject.transform.lossyScale;

                return true;
            }

            return false;
        }

        protected virtual void ResetState(State state)
        {
            state.Position = this.Toy.NetworkPosition;
            state.Rotation = this.Toy.NetworkRotation;
            state.Scale = this.Toy.NetworkScale;
        }

        protected class State
        {
            public static bool operator ==(State a, State b)
                => a?.Equals(b) ?? b is null;

            public static bool operator !=(State a, State b)
                => !(a == b);

            public Vector3 Position { get; set; }

            public LowPrecisionQuaternion Rotation { get; set; }

            public Vector3 Scale { get; set; }

            // ToDo - Add support for de-spawning objects
            public bool Visible { get; set; } = true;

            public virtual bool Equals(State other)
                => !(other is null) &&
                   this.Position.Equals(other.Position) &&
                   this.Rotation.Equals(other.Rotation) &&
                   this.Scale.Equals(other.Scale) &&
                   this.Visible == other.Visible;

            public override bool Equals(object obj)
                => obj is State other && this.Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = this.Position.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Rotation.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Scale.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Visible.GetHashCode();
                    return hashCode;
                }
            }

            public virtual void CopyValues(State other)
            {
                other.Position = this.Position;
                other.Rotation = this.Rotation;
                other.Scale = this.Scale;
                other.Visible = this.Visible;
            }
        }

        private void SyncFor(Player player, State playerState)
            => this.SyncFor(player, this.GetStateFlags(playerState));

        private void SyncFor(Player player, ulong flags)
        {
            var writer = NetworkWriterPool.GetWriter();
            var writer2 = NetworkWriterPool.GetWriter();

            if (this.Toy.netIdentity is null)
                throw new NullReferenceException("Toy.netIdentity was null");

            if (this.ToyType is null)
                throw new NullReferenceException("ToyType was null");

            if (this.ToyType == typeof(AdminToyBase))
                throw new Exception("ToyType was AdminToyBaseType");

            if ((flags & (1UL << 62)) != 0)
                return;

            MakeCustomSyncWriter.Invoke(null, new object[]
            {
                this.Toy.netIdentity,
                this.ToyType,
                null,
                this.CustomSyncVarGenerator(flags),
                writer,
                writer2,
            });

            player.ReferenceHub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage
            {
                netId = this.Toy.netIdentity.netId,
                payload = writer.ToArraySegment(),
            });

            NetworkWriterPool.Recycle(writer);
            NetworkWriterPool.Recycle(writer2);
        }

        private State GetPlayerState(Player player)
        {
            if (!this.LastStates.TryGetValue(player, out var state))
                this.LastStates[player] = state = (State)Activator.CreateInstance(this.CurrentState.GetType());

            return state;
        }

        private void LateUpdate()
        {
            if (this.Toy == null)
                return;

            if (this.CurrentState is null)
                throw new NullReferenceException("CurrentState is null");

            if (this.Controller is null)
                throw new NullReferenceException("Controller is null");

            if (!this.ShouldUpdate())
                return;

            foreach (var item in this.Controller.GetSubscribers())
                this.UpdateSubscriber(item);
        }

        private void OnDestroy()
        {
            this.Controller.RemoveScript(this);
        }
    }
}
