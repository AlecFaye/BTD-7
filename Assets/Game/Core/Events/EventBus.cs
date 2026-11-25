using System.Collections.Generic;

namespace Assets.Game.Core.Events
{ 
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> _bindings = new();

        public static void Register(EventBinding<T> binding) => _bindings.Add(binding);
        public static void Deregister(EventBinding<T> binding) => _bindings.Remove(binding);

        public static void Raise(T raisedEvent)
        {
            foreach (IEventBinding<T> binding in _bindings)
            {
                binding.OnEvent.Invoke(raisedEvent);
                binding.OnEventNoArgs.Invoke();
            }
        }

        private static void Clear()
        {
            _bindings.Clear();
        }
    }
}
