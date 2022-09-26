// -----------------------------------------------------------------------
// <copyright file="LightSynchronizerScript.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using AdminToys;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

// ReSharper disable UnusedMember.Local
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Mistaken.API.Toys
{
    internal class LightSynchronizerScript : SynchronizerScript
    {
        internal bool SyncColor { get; set; } = true;

        internal bool SyncIntensity { get; set; } = true;

        internal bool SyncRange { get; set; } = true;

        internal bool SyncShadows { get; set; } = true;

        internal Light ClonedLight
        {
            get => this.clonedLight;
            set
            {
                this.clonedLight = value;

                this.Start();
            }
        }

        protected override State CurrentState => this.currentLightState;

        protected override bool ShouldUpdate()
        {
            if (base.ShouldUpdate() ||
                (this.SyncColor && this.Light.color != this.currentLightState.Color) ||
                (this.SyncIntensity && this.Light.intensity != this.currentLightState.Intensity) ||
                (this.SyncRange && this.Light.range != this.currentLightState.Range) ||
                (this.SyncShadows && (this.Light.shadows == LightShadows.Soft) != this.currentLightState.Shadows))
            {
                this.currentLightState.Color = this.Light.color;
                this.currentLightState.Intensity = this.Light.intensity;
                this.currentLightState.Range = this.Light.range;
                this.currentLightState.Shadows = this.Light.shadows == LightShadows.Soft;

                return true;
            }

            return false;
        }

        protected override ulong GetStateFlags(State playerState)
        {
            var tor = base.GetStateFlags(playerState);

            if (!(playerState is LightState state))
                throw new ArgumentException($"Supplied {nameof(playerState)} was not {nameof(LightState)}, it was {playerState?.GetType().FullName ?? "NULL"}", nameof(playerState));

            if (!this.Light.enabled && this.Light.intensity != 0)
                Log.Warn($"Do not disable light, Set intensity to 0 instead ({this.transform.position})");

            if (this.SyncIntensity && this.currentLightState.Intensity != state.Intensity) tor += 16;
            if (this.SyncRange && this.currentLightState.Range != state.Range) tor += 32;
            if (this.SyncColor && this.currentLightState.Color != state.Color) tor += 64;
            if (this.SyncShadows && this.currentLightState.Shadows != state.Shadows) tor += 128;

            return tor;
        }

        protected override Action<NetworkWriter> CustomSyncVarGenerator(ulong flags, Action<NetworkWriter> callBackAction = null)
        {
            return base.CustomSyncVarGenerator(flags, targetWriter =>
            {
                targetWriter.WriteUInt64(flags & (16 + 32 + 64 + 128));  // intensity (16) + range (32) + color (64) + shadows (128)
                if ((flags & 16) != 0) targetWriter.WriteSingle(this.currentLightState.Intensity);
                if ((flags & 32) != 0) targetWriter.WriteSingle(this.currentLightState.Range);
                if ((flags & 64) != 0) targetWriter.WriteColor(this.currentLightState.Color);
                if ((flags & 128) != 0) targetWriter.WriteBoolean(this.currentLightState.Shadows);
                callBackAction?.Invoke(targetWriter);
            });
        }

        protected class LightState : State
        {
            public float Intensity { get; set; } = 1f;

            public float Range { get; set; } = 1f;

            public bool Shadows { get; set; }

            public Color Color { get; set; }

            public override bool Equals(State other)
                =>
                    base.Equals(other) &&
                    other is LightState light &&
                    this.Intensity.Equals(light.Intensity) &&
                    this.Range.Equals(light.Range) &&
                    this.Shadows == light.Shadows &&
                    this.Color.Equals(light.Color);

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = base.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Intensity.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Range.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Shadows.GetHashCode();
                    hashCode = (hashCode * 397) ^ this.Color.GetHashCode();
                    return hashCode;
                }
            }

            public override void CopyValues(State other)
            {
                base.CopyValues(other);

                if (!(other is LightState primitive))
                    throw new ArgumentException("Expected argument of type " + nameof(LightState), nameof(other));

                primitive.Color = this.Color;
                primitive.Intensity = this.Intensity;
                primitive.Shadows = this.Shadows;
                primitive.Range = this.Range;
            }
        }

        private readonly LightState currentLightState = new LightState();
        private Light clonedLight;

        private Light Light => this.ClonedLight ?? (this.Toy as LightSourceToy)?._light;

        private void Start()
        {
            base.ShouldUpdate();
            this.currentLightState.Color = this.Light.color;
            this.currentLightState.Intensity = this.Light.intensity;
            this.currentLightState.Range = this.Light.range;
            this.currentLightState.Shadows = this.Light.shadows == LightShadows.Soft;

            foreach (var player in this.Controller.GetSubscribers())
                this.UpdateSubscriber(player);
        }
    }
}
