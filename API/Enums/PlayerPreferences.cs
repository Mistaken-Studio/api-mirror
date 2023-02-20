using JetBrains.Annotations;
using System;

namespace Mistaken.API.Enums;

/// <summary>
/// Player preferences enum.
/// </summary>
[PublicAPI]
[Flags]
public enum PlayerPreferences : ushort
{
    None = 0,
    DisableColorfulEzSpectatorScp079 = 1,
    DisableTransscript = 2,
    DisableFastRoundRestart = 4,
}
