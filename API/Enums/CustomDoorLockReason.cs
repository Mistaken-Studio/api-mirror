using System;

namespace Mistaken.API.Enums;

/// <summary>
/// Door lock reason.
/// </summary>
[Flags]
public enum CustomDoorLockReason : ushort
{
    Cooldown = 512,
    BlockedBySomething = 1024,
    RequirementsNotMet = 2048,
}
