namespace Mistaken.API.Events;

public delegate void CustomEventHandler<T>(T ev) where T : System.EventArgs;
