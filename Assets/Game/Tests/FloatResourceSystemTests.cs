using Assets.Game.Core.ResourceSystems;
using NUnit.Framework;

namespace Assets.Game.Tests
{
    [TestFixture]
    public class FloatResourceSystemTests
    {
        private const float Tolerance = 1e-5f;
        private int _changedCount;

        [SetUp]
        public void SetUp() => _changedCount = 0;

        private FloatResourceSystem Create(float current, float max)
        {
            FloatResourceSystem system = new(current, max);
            system.OnChanged += () => _changedCount++;
            return system;
        }

        // ---------- Constructors ----------

        [Test]
        public void Constructor_SingleArg_SetsMaxEqualToCurrent()
        {
            FloatResourceSystem system = new(40.5f);

            Assert.That(system.Current, Is.EqualTo(40.5f));
            Assert.That(system.Max, Is.EqualTo(40.5f));
        }

        [Test]
        public void Constructor_TwoArgs_StoresBothValues()
        {
            FloatResourceSystem system = new(30.5f, 100f);

            Assert.That(system.Current, Is.EqualTo(30.5f));
            Assert.That(system.Max, Is.EqualTo(100f));
        }

        // ---------- Add ----------

        [TestCase(50f, 100f, 10.5f, 60.5f)]
        [TestCase(0f, 100f, 100f, 100f)]
        [TestCase(99.9f, 100f, 0.1f, 100f)]
        [TestCase(90f, 100f, 50f, 100f)]
        [TestCase(100f, 100f, 1f, 100f)]
        [TestCase(50f, 100f, float.PositiveInfinity, 100f)]
        public void Add_PositiveAmount_IncreasesAndClampsToMax(float start, float max, float amount, float expected)
        {
            FloatResourceSystem system = Create(start, max);

            system.Add(amount);

            Assert.That(system.Current, Is.EqualTo(expected).Within(Tolerance));
        }

        [TestCase(0f)]
        [TestCase(-0.5f)]
        [TestCase(-100f)]
        [TestCase(float.NegativeInfinity)]
        public void Add_ZeroOrNegative_IsIgnoredAndDoesNotNotify(float amount)
        {
            FloatResourceSystem system = Create(50f, 100f);

            system.Add(amount);

            Assert.That(system.Current, Is.EqualTo(50f));
            Assert.That(_changedCount, Is.EqualTo(0));
        }

        [Test]
        public void Add_Success_NotifiesOnce()
        {
            FloatResourceSystem system = Create(50f, 100f);

            system.Add(10f);

            Assert.That(_changedCount, Is.EqualTo(1));
        }

        // ---------- TryWithdraw ----------

        [Test]
        public void TryWithdraw_LessThanCurrent_ReturnsTrueAndReduces()
        {
            FloatResourceSystem system = Create(50f, 100f);

            bool result = system.TryWithdraw(20.25f);

            Assert.That(result, Is.True);
            Assert.That(system.Current, Is.EqualTo(29.75f).Within(Tolerance));
        }

        [Test]
        public void TryWithdraw_ExactlyCurrent_ReturnsTrueAndEmpties()
        {
            FloatResourceSystem system = Create(50f, 100f);

            Assert.That(system.TryWithdraw(50f), Is.True);
            Assert.That(system.Current, Is.EqualTo(0f));
        }

        [TestCase(50.001f)]
        [TestCase(float.PositiveInfinity)]
        public void TryWithdraw_MoreThanCurrent_ReturnsFalseAndLeavesStateUnchanged(float amount)
        {
            FloatResourceSystem system = Create(50f, 100f);

            bool result = system.TryWithdraw(amount);

            Assert.That(result, Is.False);
            Assert.That(system.Current, Is.EqualTo(50f));
            Assert.That(_changedCount, Is.EqualTo(0));
        }

        [Test]
        public void TryWithdraw_FromEmpty_ReturnsFalse()
        {
            FloatResourceSystem system = Create(0f, 100f);

            Assert.That(system.TryWithdraw(0.1f), Is.False);
            Assert.That(system.Current, Is.EqualTo(0f));
        }

        [Test]
        public void TryWithdraw_Negative_ReturnsFalseAndDoesNotNotify()
        {
            FloatResourceSystem system = Create(50f, 100f);

            Assert.That(system.TryWithdraw(-1f), Is.False);
            Assert.That(system.Current, Is.EqualTo(50f));
            Assert.That(_changedCount, Is.EqualTo(0));
        }

        [Test]
        public void TryWithdraw_Success_NotifiesOnce()
        {
            FloatResourceSystem system = Create(50f, 100f);

            system.TryWithdraw(10f);

            Assert.That(_changedCount, Is.EqualTo(1));
        }

        [Test]
        public void TryWithdraw_Zero_CurrentlyReturnsTrueAndNotifies()
        {
            FloatResourceSystem system = Create(50f, 100f);

            Assert.That(system.TryWithdraw(0f), Is.True);
            Assert.That(system.Current, Is.EqualTo(50f));
            Assert.That(_changedCount, Is.EqualTo(1));
        }

        // ---------- Normalized ----------

        [TestCase(0f, 100f, 0f)]
        [TestCase(25f, 100f, 0.25f)]
        [TestCase(50f, 100f, 0.5f)]
        [TestCase(100f, 100f, 1f)]
        [TestCase(150f, 100f, 1f)]    // clamped above
        [TestCase(-10f, 100f, 0f)]    // clamped below
        public void Normalized_ReturnsClampedCurrentOverMax(float current, float max, float expected)
        {
            FloatResourceSystem system = new(current, max);

            Assert.That(system.Normalized, Is.EqualTo(expected).Within(Tolerance));
        }

        [TestCase(0f)]
        [TestCase(-10f)]
        public void Normalized_MaxZeroOrNegative_ReturnsZero(float max)
        {
            FloatResourceSystem system = new(5f, max);

            Assert.That(system.Normalized, Is.EqualTo(0f));
        }

        [Test]
        public void Normalized_UpdatesAfterAddAndWithdraw()
        {
            FloatResourceSystem system = Create(50f, 100f);

            system.Add(25f);
            Assert.That(system.Normalized, Is.EqualTo(0.75f).Within(Tolerance));

            system.TryWithdraw(50f);
            Assert.That(system.Normalized, Is.EqualTo(0.25f).Within(Tolerance));
        }

        // ---------- OnChanged / NotifyChanged ----------

        [Test]
        public void NotifyChanged_InvokesAllSubscribers()
        {
            FloatResourceSystem system = new(10f, 10f);
            int a = 0;
            int b = 0;

            system.OnChanged += () => a++;
            system.OnChanged += () => b++;

            system.NotifyChanged();

            Assert.That(a, Is.EqualTo(1));
            Assert.That(b, Is.EqualTo(1));
        }

        // ---------- Sequences ----------

        [Test]
        public void AddThenWithdraw_RoundTrip_RestoresValue()
        {
            FloatResourceSystem system = Create(50f, 100f);

            system.Add(30f);
            system.TryWithdraw(30f);

            Assert.That(system.Current, Is.EqualTo(50f).Within(Tolerance));
            Assert.That(_changedCount, Is.EqualTo(2));
        }

        [Test]
        public void FailedOperations_DoNotCorruptState()
        {
            FloatResourceSystem system = Create(50f, 100f);

            system.TryWithdraw(999f);
            system.Add(-5f);
            system.TryWithdraw(-1f);

            Assert.That(system.Current, Is.EqualTo(50f));
            Assert.That(_changedCount, Is.EqualTo(0));
        }
    }
}
