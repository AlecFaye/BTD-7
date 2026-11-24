namespace Assets.Game.Core.ResourceSystems
{
    public class IntResourceSystem : ResourceSystem<int>
    {
        public IntResourceSystem(int current) : base(current) { }

        public IntResourceSystem(int current, int max) : base(current, max) { }

        public override float Normalized => Max > 0 ? UnityEngine.Mathf.Clamp((float)Current / Max, 0f, 1f) : 0f;

        public override bool TryWithdraw(int amount)
        {
            if (amount < 0 || Current < amount) 
                return false;

            Current -= amount;
            NotifyChanged();

            return true;
        }

        public override void Add(int amount)
        {
            if (amount <= 0) 
                return;

            if (amount > Max - Current)
                Current = Max;
            else
                Current = UnityEngine.Mathf.Clamp(Current + amount, 0, Max);

            NotifyChanged();
        }
    }
}
