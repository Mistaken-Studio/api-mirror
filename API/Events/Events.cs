using Exiled.Events.Extensions;

namespace Mistaken.API.Events
{
    /// <summary>
    /// Mistaken custom events.
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// The event is fired when all plugins are loaded and initialized.
        /// </summary>
        public static event Exiled.Events.Events.CustomEventHandler PostInitializationEvent;

        /// <summary>
        /// The event is fired just before WaitingForPlayers.
        /// </summary>
        public static event Exiled.Events.Events.CustomEventHandler PreWaitingForPlayersEvent;

        internal static void OnPostInitialization()
        {
            PostInitializationEvent.InvokeSafely();
        }

        internal static void OnPreWaitingForPlayersEvent()
        {
            PreWaitingForPlayersEvent.InvokeSafely();
        }
    }
}