using PainKiller.CommandPrompt.CoreLib.Core.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Core.Services;
public class EventBusService : IEventBusService
{
    private EventBusService(){}

    private static readonly Lazy<IEventBusService> Lazy = new(() => new EventBusService());
    public static IEventBusService Service => Lazy.Value;

    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        if (!_subscribers.TryGetValue(typeof(TEvent), out var handlers))
        {
            handlers = [];
            _subscribers[typeof(TEvent)] = handlers;
        }
        handlers.Add(handler);
    }
    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        if (_subscribers.TryGetValue(typeof(TEvent), out var handlers)) handlers.Remove(handler);
    }
    public void Publish<TEvent>(TEvent eventData)
    {
        if (!_subscribers.TryGetValue(typeof(TEvent), out var handlers)) return;
        foreach (var handler in handlers) ((Action<TEvent>)handler)?.Invoke(eventData);
    }
}