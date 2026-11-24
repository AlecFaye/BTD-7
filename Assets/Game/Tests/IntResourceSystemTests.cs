using Assets.Game.Core.ResourceSystems;
using NUnit.Framework;

namespace Assets.Game.Tests
{
    [TestFixture]
    public class IntResourceSystemTests
    {
        private int _changedCount;

        [SetUp]
        public void SetUp() => _changedCount = 0;

        private IntResourceSystem Create(int current, int max)
        {
            IntResourceSystem system = new(current, max);
            system.OnChanged += () => _changedCount++;
            return system;
        }

        // ---------- Constructors ----------

        [Test]
        public void Constructor_SingleArg_SetsMaxEqualToCurrent()
        {
            IntResourceSystem system = new(40);

            Assert.That(system.Current, Is.EqualTo(40));
            Assert.That(system.Max, Is.EqualTo(40));
        }

        [Test]
        public void Constructor_TwoArgs_StoresBothValues()
        {
            IntResourceSystem system = new(30, 100);

            Assert.That(system.Current, Is.EqualTo(30));
            Assert.That(system.Max, Is.EqualTo(100));
        }

        // ---------- Add ----------

        [TestCase(50, 100, 10, 60)]
        [TestCase(0, 100, 100, 100)]    // exactly reaches max
        [TestCase(90, 100, 10, 100)]    // exactly reaches max
        [TestCase(90, 100, 50, 100)]    // clamped to max
        [TestCase(100, 100, 1, 100)]    // already full
        public void Add_PositiveAmount_IncreasesAndClampsToMax(int start, int max, int amount, int expected)
        {
            IntResourceSystem system = Create(start, max);

            system.Add(amount);

            Assert.That(system.Current, Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public void Add_ZeroOrNegative_IsIgnoredAndDoesNotNotify(int amount)
        {
            IntResourceSystem system = Create(50, 100);

            system.Add(amount);

            Assert.That(system.Current, Is.EqualTo(50));
            Assert.That(_changedCount, Is.EqualTo(0));
        }

        [Test]
        public void Add_Success_NotifiesOnce()
        {
            IntResourceSystem system = Create(50, 100);

            system.Add(10);

            Assert.That(_changedCount, Is.EqualTo(1));
        }

        [Test]
        public void Add_WhenCurrentAboveMax_ClampsDownToMax()
        {
            IntResourceSystem system = Create(150, 100);

            system.Add(1);

            Assert.That(system.Current, Is.EqualTo(100));
        }

        // ---------- TryWithdraw ----------

        [Test]
        public void TryWithdraw_LessThanCurrent_ReturnsTrueAndReduces()
        {
            IntResourceSystem system = Create(50, 100);

            var result = system.TryWithdraw(20);

            Assert.That(result, Is.True);
            Assert.That(system.Current, Is.EqualTo(30));
        }

        [Test]
        public void TryWithdraw_ExactlyCurrent_ReturnsTrueAndEmpties()
        {
            IntResourceSystem system = Create(50, 100);

            bool result = system.TryWithdraw(50);

            Assert.That(result, Is.True);
            Assert.That(system.Current, Is.EqualTo(0));
        }

        [Test]
        public void TryWithdraw_MoreThanCurrent_ReturnsFalseAndLeavesStateUnchanged()
        {
            IntResourceSystem system = Create(50, 100);

            bool result = system.TryWithdraw(51);

            Assert.That(result, Is.False);
            Assert.That(system.Current, Is.EqualTo(50));
            Assert.That(_changedCount, Is.EqualTo(0));
        }

        [Test]
        public void TryWithdraw_FromEmpty_ReturnsFalse()
        {
            IntResourceSystem system = Create(0, 100);

            Assert.That(system.TryWithdraw(1), Is.False);
            Assert.That(system.Current, Is.EqualTo(0));
        }

        [Test]
        public void TryWithdraw_Negative_ReturnsFalseAndDoesNotNotify()
        {
            var system = Create(50, 100);

            var result = system.TryWithdraw(-5);

            Assert.That(result, Is.False);
            Assert.That(system.Current, Is.EqualTo(50));
            Assert.That(_changedCount, Is.EqualTo(0));
        }

        [Test]
        public void TryWithdraw_Success_NotifiesOnce()
        {
            IntResourceSystem system = Create(50, 100);

            system.TryWithdraw(10);

            Assert.That(_changedCount, Is.EqualTo(1));
        }

        [Test]
        public void TryWithdraw_Zero_CurrentlyReturnsTrueAndNotifies()
        {
            IntResourceSystem system = Create(50, 100);

            bool result = system.TryWithdraw(0);

            Assert.That(result, Is.True);
            Assert.That(system.Current, Is.EqualTo(50));
            Assert.That(_changedCount, Is.EqualTo(1));
        }

        // ---------- Normalized ----------

        [TestCase(0, 100, 0f)]
        [TestCase(25, 100, 0.25f)]
        [TestCase(50, 100, 0.5f)]
        [TestCase(100, 100, 1f)]
        public void Normalized_ReturnsCurrentOverMax(int current, int max, float expected)
        {
            IntResourceSystem system = new(current, max);

            Assert.That(system.Normalized, Is.EqualTo(expected).Within(1e-5f));
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void Normalized_MaxZeroOrNegative_ReturnsZero(int max)
        {
            IntResourceSystem system = new(5, max);

            Assert.That(system.Normalized, Is.EqualTo(0f));
        }

        [Test]
        public void Normalized_UpdatesAfterAddAndWithdraw()
        {
            IntResourceSystem system = Create(50, 100);

            system.Add(25);
            Assert.That(system.Normalized, Is.EqualTo(0.75f).Within(1e-5f));

            system.TryWithdraw(50);
            Assert.That(system.Normalized, Is.EqualTo(0.25f).Within(1e-5f));
        }

        // ---------- OnChanged / NotifyChanged ----------

        [Test]
        public void NotifyChanged_InvokesAllSubscribers()
        {
            IntResourceSystem system = new(10, 10);
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
            IntResourceSystem system = Create(50, 100);

            system.Add(30);
            system.TryWithdraw(30);

            Assert.That(system.Current, Is.EqualTo(50));
            Assert.That(_changedCount, Is.EqualTo(2));
        }

        [Test]
        public void RepeatedWithdraw_UntilEmpty_ThenFails()
        {
            IntResourceSystem system = Create(30, 100);

            Assert.That(system.TryWithdraw(10), Is.True);
            Assert.That(system.TryWithdraw(10), Is.True);
            Assert.That(system.TryWithdraw(10), Is.True);
            Assert.That(system.TryWithdraw(1), Is.False);
            Assert.That(system.Current, Is.EqualTo(0));
        }

        [Test]
        public void FailedOperations_DoNotCorruptState()
        {
            IntResourceSystem system = Create(50, 100);

            system.TryWithdraw(999);
            system.Add(-5);
            system.TryWithdraw(-1);

            Assert.That(system.Current, Is.EqualTo(50));
            Assert.That(_changedCount, Is.EqualTo(0));
        }
    }
}
