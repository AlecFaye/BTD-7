namespace Assets.Game.Core.ResourceSystems
{
    public interface IResourceSystem
    {
        public float Normalized { get; }
        public event System.Action OnChanged;
    }
}
