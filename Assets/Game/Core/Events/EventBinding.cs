using System;

namespace Assets.Game.Core.Events
{
    internal interface IEventBinding<T>
    {
        public System.Action<T> OnEvent { get; set; }
        public System.Action OnEventNoArgs { get; set; }
    }

    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> _onEvent = _ => { };
        private Action _onEventNoArgs = () => { };

        public Action<T> OnEvent 
        { 
            get => _onEvent; 
            set => _onEvent = value; 
        }

        public Action OnEventNoArgs 
        { 
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        public EventBinding(Action<T> onEvent) => _onEvent = onEvent;
        public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs;

        public void Add(Action<T> onEvent) => OnEvent += onEvent;
        public void Remove(Action<T> onEvent) => OnEvent -= onEvent;

        public void Add(Action onEvent) => OnEventNoArgs += onEvent;
        public void Remove(Action onEvent) => OnEventNoArgs -= onEvent;
    }
}
