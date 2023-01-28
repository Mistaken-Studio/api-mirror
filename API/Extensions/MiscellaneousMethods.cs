using PluginAPI.Core;
using System.Collections.Generic;

namespace Mistaken.API.Extensions;

public static class MiscellaneousMethods
{
    public static string GetServerName()
    {
        if (!_serverNames.ContainsKey(Server.Port))
            return "Unknown";

        return _serverNames[Server.Port];
    }

    private static readonly Dictionary<ushort, string> _serverNames = new()
    {
        { 7777, "#1 RP Vanilla" },
        { 7778, "#2 RP" },
        { 7779, "#3 Casual" },
        { 7780, "#4 Memes" },
        { 7790, "#Test 1" },
        { 7791, "#Test 2" },
        { 8000, "#Beta 1" },
    };
}
