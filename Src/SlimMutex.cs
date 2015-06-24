using System;
using System.Threading;

namespace XTypes
{
    /// <summary>
    /// Mutually exclusive lock and synchoronisation object that limits the number of threads that can access a resource or pool 
    /// of resources concurrently.
    /// </summary>
    /// <remarks>
    internal sealed class SlimMutex
    {
        private static readonly Releaser NullReleaser = new Releaser();

        /// <summary>
        /// Gets if the <see cref="SlimMutex"/> is entered or not.
        /// </summary>
        public bool IsEntered
        {
            get { lock (@lock) { return _entered; } }
        }
        /// <summary>
        /// Gets the number of threads waiting to enter the <see cref="SlimMutex"/>.
        /// </summary>
        public int Waiting
        {
            get { lock (@lock) { return _waiting; } }
        }

        private bool _entered;
        private int _waiting;
        private readonly object @lock = new object();

        /// <summary>
        /// Attemps to enter and aquire exclusive access.Blocks and queues for access until all preceeding owners have exited.
        /// </summary>
        /// <remarks>
        /// This method blocks until the calling thread enters the lock, and therefore might never return.
        /// </remarks>
        /// <returns>An <see cref="IDisposable"/> handle which will release the lock when disposed.</returns>
        public IDisposable Enter()
        {
            lock (@lock)
            {
                _waiting++;

                while (_entered)
                {
                    Monitor.Wait(@lock);
                }

                _entered = true;
                _waiting--;

                Assert.GreaterOrEqual(_waiting, 0);

                return new Releaser(Exit);
            }
        }
        /// <summary>
        /// Attemps to enter and aquire exclusive access. Blocks and queues for access until all preceeding owners have exited,
        /// or timeout occurs.
        /// </summary>
        /// <param name="timeout">Span of time allowed before attempting to aquire the lock fails and control is returned.</param>
        /// <param name="success">True if the lock is entered; False if the attempt to aquire the lock timed out.</param>
        /// <remarks>
        /// This method blocks until the calling thread enters the lock, or timeout occurs.
        /// </remarks>
        /// <returns></returns>
        public IDisposable TryEnter(TimeSpan timeout, out bool success)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            lock (@lock)
            {
                timer.Start();

                _waiting++;

                while (_entered)
                {
                    TimeSpan maxWait = timeout - timer.Elapsed;
                    if (maxWait <= TimeSpan.Zero || !Monitor.Wait(@lock, maxWait))
                    {
                        _waiting--;
                        success = false;
                        return NullReleaser;
                    }
                }

                _entered = true;
                _waiting--;

                timer.Stop();

                Assert.GreaterOrEqual(_waiting, 0);

                success = true;
                return new Releaser(Exit);
            }
        }

        private void Exit()
        {
            lock (@lock)
            {
                _entered = false;
                Monitor.Pulse(@lock);
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
