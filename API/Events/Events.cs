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
        public static event System.Action PostInitializationEvent;

        /// <summary>
        /// The event is fired just before WaitingForPlayers.
        /// </summary>
        public static event System.Action PreWaitingForPlayersEvent;

        internal static void OnPostInitialization()
        {
            PostInitializationEvent?.Invoke();
        }

        internal static void OnPreWaitingForPlayers()
        {
            PreWaitingForPlayersEvent?.Invoke();
        }
    }
}