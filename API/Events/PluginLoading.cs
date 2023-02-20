using System;

namespace Mistaken.API.Events;

/// <summary>
/// Mistaken events for helping with loading plugins.
/// </summary>
public static class PluginLoading
{
    /// <summary>
    /// The event is fired when all plugins are loaded and initialized.
    /// </summary>
    public static event Action PostInitialization;

    /// <summary>
    /// Invokes <see cref="PostInitialization"/>.
    /// </summary>
    public static void InvokePostInitialization()
        => PostInitialization?.Invoke();
}