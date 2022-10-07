// -----------------------------------------------------------------------
// <copyright file="BetterWarheadHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Mistaken.API
{
    /// <summary>
    /// Obsolete.
    /// </summary>
    [PublicAPI]
    [System.Obsolete("Moved to Mistaken.API.API", true)]
    public class BetterWarheadHandler
    {
        /// <summary>
        /// Gets <see cref="WarheadStatus"/> instance.
        /// </summary>
        public static WarheadStatus Warhead => new (Handlers.BetterWarheadHandler.Warhead);

        /// <summary>
        /// Warhead status.
        /// </summary>
        [PublicAPI]
        [System.Obsolete("Moved to Mistaken.API.API", true)]
        public class WarheadStatus
        {
            /// <inheritdoc cref="Warhead.IsInProgress"/>
            public bool CountingDown
            {
                get => Exiled.API.Features.Warhead.IsInProgress;
                set
                {
                    if (value)
                        Exiled.API.Features.Warhead.Start();
                    else
                        Exiled.API.Features.Warhead.Stop();
                }
            }

            /// <inheritdoc cref="Warhead.DetonationTimer"/>
            public float TimeLeft => Exiled.API.Features.Warhead.DetonationTimer;

            /// <summary>
            /// Gets or sets a value indicating whether lever should be locked or not.
            /// </summary>
            public bool LeverLock { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether warhead can be stopped using button or not.
            /// </summary>
            public bool StopLock { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether warhead can be started using button or not.
            /// </summary>
            public bool StartLock { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether surface button can be unlocked or not.
            /// </summary>
            public bool ButtonLock { get; set; }

            /// <inheritdoc cref="Warhead.IsKeycardActivated"/>
            public bool ButtonOpen
            {
                get => Exiled.API.Features.Warhead.IsKeycardActivated;
                set => Exiled.API.Features.Warhead.IsKeycardActivated = value;
            }

            /// <inheritdoc cref="Warhead.IsDetonated"/>
            public bool Detonated => Exiled.API.Features.Warhead.IsDetonated;

            /// <inheritdoc cref="Warhead.LeverStatus"/>
            public bool Enabled
            {
                get => Exiled.API.Features.Warhead.LeverStatus;
                set => Exiled.API.Features.Warhead.LeverStatus = value;
            }

            /// <summary>
            /// Gets last user that used started warhead.
            /// </summary>
            public Player LastStartUser { get; internal set; }

            /// <summary>
            /// Gets last user that used stopped warhead.
            /// </summary>
            public Player LastStopUser { get; internal set; }

            internal WarheadStatus(Handlers.BetterWarheadHandler.WarheadStatus other)
            {
                this.LeverLock = other.LeverLock;
                this.StopLock = other.StopLock;
                this.StartLock = other.StartLock;
                this.ButtonLock = other.ButtonLock;

                this.LastStartUser = other.LastStartUser;
                this.LastStopUser = other.LastStopUser;
            }
        }
    }
}
