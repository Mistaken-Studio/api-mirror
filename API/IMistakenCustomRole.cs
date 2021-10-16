// -----------------------------------------------------------------------
// <copyright file="IMistakenCustomRole.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Mistaken.API
{
    /// <summary>
    /// Interace used to mark Mistaken's Custom Roles.
    /// </summary>
    public interface IMistakenCustomRole
    {
        /// <summary>
        /// Gets custom role.
        /// </summary>
        MistakenCustomRoles CustomRole { get; }
    }
}
