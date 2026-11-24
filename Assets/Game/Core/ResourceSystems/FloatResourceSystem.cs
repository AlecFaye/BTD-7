namespace Assets.Game.Core.ResourceSystems
{
    public class FloatResourceSystem : ResourceSystem<float>
    {
        public FloatResourceSystem(float current) : base(current) { }

        public FloatResourceSystem(float current, float max) : base(current, max) { }

        public override float Normalized => Max > 0f ? UnityEngine.Mathf.Clamp(Current / Max, 0f, 1f) : 0f;

        public override bool TryWithdraw(float amount)
        {
            if (amount < 0f || Current < amount || float.IsNaN(amount)) 
                return false;

            Current -= amount;
            NotifyChanged();

            return true;
        }

        public override void Add(float amount)
        {
            if (amount <= 0f || float.IsNaN(amount)) 
                return;

            Current = UnityEngine.Mathf.Clamp(Current + amount, 0f, Max);
            NotifyChanged();
        }
    }
}
