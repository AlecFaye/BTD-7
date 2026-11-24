namespace Assets.Game.Core.ResourceSystems
{
    public abstract class ResourceSystem<T> : IResourceSystem 
        where T : struct, System.IComparable<T>
    {
        public T Current { get; protected set; }
        public T Max { get; protected set; }

        public abstract float Normalized { get; }
        public event System.Action OnChanged = delegate { };

        public ResourceSystem(T current)
        {
            Current = current;
            Max = current;
        }
        
        public ResourceSystem(T current, T max)
        {
            Current = current;
            Max = max;
        }

        public void NotifyChanged()
        {
            OnChanged?.Invoke();
        }

        public abstract bool TryWithdraw(T amount);
        public abstract void Add(T amount);
    }
}
