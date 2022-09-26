// -----------------------------------------------------------------------
// <copyright file="PrimitiveSynchronizerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using AdminToys;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Mistaken.API.Toys
{
    internal class PrimitiveSynchronizerScript : SynchronizerScript
    {
        internal MeshRenderer MeshRenderer { get; set; } = null;

        internal bool SyncColor => !(this.MeshRenderer is null);

        protected override State CurrentState => this.currentPrimitiveState;

        protected override bool ShouldUpdate()
        {
            if (!base.ShouldUpdate() && this.MeshRenderer?.material.color == this.currentPrimitiveState.Color)
                return false;

            this.currentPrimitiveState.Color = this.MeshRenderer?.material.color ?? Color.magenta;
            return true;
        }

        protected override ulong GetStateFlags(State playerState)
        {
            var tor = base.GetStateFlags(playerState);

            if (!(playerState is PrimitiveState state))
                throw new ArgumentException($"Supplied {nameof(playerState)} was not {nameof(PrimitiveState)}, it was {playerState?.GetType().FullName ?? "NULL"}", nameof(playerState));

            if (this.SyncColor && this.currentPrimitiveState.Color != state.Color) tor += 32;
            return tor;
        }

        protected override Action<NetworkWriter> CustomSyncVarGenerator(ulong flags, Action<NetworkWriter> callBackAction = null)
        {
            return base.CustomSyncVarGenerator(flags, targetWriter =>
            {
                targetWriter.WriteUInt64(flags & 32UL); // color (32) | flags & (~31UL)
                if ((flags & 32) != 0) targetWriter.WriteColor(this.currentPrimitiveState.Color);
                callBackAction?.Invoke(targetWriter);
            });
        }

        protected class PrimitiveState : State
        {
            public override bool Equals(State other)
                =>
                    base.Equals(other) &&
                    other is PrimitiveState primitive &&
                    this.Visible == primitive.Visible &&
                    this.Color == primitive.Color;

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = base.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Visible.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Color.GetHashCode();
                    return hashCode;
                }
            }

            public override void CopyValues(State other)
            {
                base.CopyValues(other);

                if (!(other is PrimitiveState primitive))
                    throw new ArgumentException("Expected argument of type " + nameof(PrimitiveState), nameof(other));

                primitive.Color = this.Color;
                primitive.Visible = this.Visible;
            }

            // ToDo - Add support for de-spawning objects
            public bool Visible { get; set; }

            public Color Color { get; set; }
        }

        private readonly PrimitiveState currentPrimitiveState = new PrimitiveState();
    }
}
