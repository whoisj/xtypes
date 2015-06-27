using System;
using System.Threading;


namespace XTypes
{
    public class SlimSemaphore
    {
        public SlimSemaphore(int initialCapacity, int initialCount)
        {
            Ensure.Greater(initialCapacity, 0, nameof(initialCapacity));
            Ensure.GreaterOrEqual(initialCount, 0, nameof(initialCount));

            _capacity = initialCapacity;
            _entered = initialCount;
        }
        public SlimSemaphore(int initialCapacity)
            : this(initialCapacity, 0)
        { }
        public SlimSemaphore()
            : this(1, 0)
        { }

        public int Capacity { get { lock (@lock) { return _capacity; } } }
        public int Entered { get { lock (@lock) { return _entered; } } }
        public int Waiting { get { lock (@lock) { return _waiting; } } }

        private int _capacity;
        private int _entered;
        private int _waiting;
        private readonly object @lock = new object();

        public int Decrement()
        {
            lock (@lock)
            {
                _waiting++;

                while (_entered <= 0)
                {
                    Monitor.Wait(@lock);
                }

                _entered--;
                _waiting--;

                Monitor.PulseAll(@lock);

                return _entered - _capacity;
            }
        }

        public IDisposable Enter()
        {
            lock (@lock)
            {
                Decrement();

                return new Releaser(Exit);
            }
        }

        public int Increment()
        {
            lock (@lock)
            {
                _waiting++;

                while (_capacity <= _entered)
                {
                    Monitor.Wait(@lock);
                }

                _entered++;
                _waiting--;

                Monitor.PulseAll(@lock);

                return _entered - _capacity;
            }
        }

        public void SetCapacity(int newCapacity)
        {
            lock (@lock)
            {
                while (newCapacity < _entered)
                {
                    Monitor.Wait(@lock);
                }

                int delta = newCapacity - _capacity;
                _capacity = newCapacity;

                if (delta > 0)
                {
                    Monitor.PulseAll(@lock);
                }
            }
        }

        private void Exit()
        {
            lock (@lock)
            {
                Increment();

                Monitor.PulseAll(@lock);
            }
        }

        private struct Releaser : IDisposable
        {
            private ReleaseDelegate _release;

            public Releaser(ReleaseDelegate release)
            {
                Assert.NotNull(release);
                _release = release;
            }

            public void Dispose()
            {
                if (_release != null)
                {
                    _release();
                    _release = null;
                }
            }
        }

        private delegate void ReleaseDelegate();
    }
}
