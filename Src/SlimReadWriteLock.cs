using System;
using System.Threading;

namespace XTypes
{
    /// <summary>
    /// A lock that and synchoronisation object that is used to manage access to a resource or pool of resources, allowing 
    /// concurrent reading access or exclusive writing access.
    /// </summary>
    internal sealed class SlimReadWriteLock
    {
        private static readonly Releaser NullReleaser = new Releaser();

        /// <summary>
        /// Gets the count of readers currently entered into the <see cref="SlimReadWriteLock"/>.
        /// </summary>
        public int EnteredReaders
        {
            get { lock (@lock) { return Math.Max(_state, 0); } }
        }
        /// <summary>
        /// Gets the number of writers concurrently entered into the <see cref="SlimReadWriteLock"/>; expected [0, 1].
        /// </summary>
        internal int EnteredWriters
        {
            get { lock (@lock) { return -Math.Min(_state, 0); } }
        }
        /// <summary>
        /// Gets if the <see cref="SlimReadWriteLock"/> is read lock entered.
        /// </summary>
        public bool IsReadLockEntered
        {
            get { lock (@lock) { return _state > 0; } }
        }
        /// <summary>
        /// Gets if the <see cref="SlimReadWriteLock"/> is write lock is entered.
        /// </summary>
        public bool IsWriteLockEntered
        {
            get { lock (@lock) { return _state < 0; } }
        }
        /// <summary>
        /// Gets the count of pending reader lock requests in the <see cref="SlimReadWriteLock"/>.
        /// </summary>
        public int WaitingReaders
        {
            get { lock (@lock) { return _waitingReaders; } }
        }
        /// <summary>
        /// Gets the count of pending writer lock requests in the <see cref="SlimReadWriteLock"/>.
        /// </summary>
        public int WaitingWriters
        {
            get { lock (@lock) { return _waitingWriters; } }
        }

        private int _state;
        private int _waitingReaders;
        private int _waitingWriters;
        private readonly object @lock = new object();

        /// <summary>
        /// Attempts to enter and aquire read access. Blocks and queues for accuess until all write locks have exited.
        /// </summary>
        /// <remarks>
        /// Multiple threads can enter read lock concurrently.
        /// 
        /// This method blocks until the calling thread enters the lock, and therefore might never return.
        /// 
        /// If one or more threads are waiting to enter write lock, a thread that calls the <see cref="EnterReadLock"/> method 
        /// blocks until those threads have entered and then exited from it.
        /// </remarks>
        /// <returns>An <see cref="IDisposable"/> handle which will release the lock when disposed.</returns>
        public IDisposable EnterReadLock()
        {
            System.Diagnostics.Trace.TraceWarning($"{typeof(SlimReadWriteLock)}: {nameof(EnterReadLock)} enter @ {DateTime.Now}");

            lock (@lock)
            {
                _waitingReaders++;

                while (_state < 0 || _waitingWriters > 0)
                {
                    Monitor.Wait(@lock);
                }

                _state++;
                _waitingReaders--;

                Assert.Greater(_state, 0);
                Assert.GreaterOrEqual(_waitingReaders, 0);

                System.Diagnostics.Trace.TraceWarning($"{typeof(SlimReadWriteLock)}: {nameof(EnterReadLock)} exit @ {DateTime.Now}");

                return new Releaser(ExitReadLock);
            }
        }
        /// <summary>
        /// Attemps to enter and aquire exclusive write access. Blocks and queues for access until all preceeding locks have exited.
        /// </summary>
        /// <remarks>
        /// Write locks are exclusive, and no other locks can be taken while a write lock is held.
        /// 
        /// This method blocks until the calling thread enters the lock, and therefore might never return.
        /// 
        /// If other threads have entered the lock in read mode, a thread that calls the <see cref="EnterWriteLock"/> method 
        /// blocks until those threads have exited read mode. When there are threads waiting to enter write mode, additional 
        /// threads that try to enter read mode block until all the threads waiting to enter write mode have entered write mode 
        /// and then exited from it.
        /// </remarks>
        /// <returns>An <see cref="IDisposable"/> handle which will release the lock when disposed.</returns>
        public IDisposable EnterWriteLock()
        {
            lock (@lock)
            {
                _waitingWriters++;

                while (_state != 0)
                {
                    Monitor.Wait(@lock);
                }

                _state = -1;
                _waitingWriters--;

                Assert.GreaterOrEqual(_waitingWriters, 0);

                return new Releaser(ExitWriteLock);
            }
        }
        /// <summary>
        /// Attempts to enter and aquire read access. Blocks and queues for accuess until all write locks have exited or timeout occurs.
        /// </summary>
        /// <param name="timeout">Span of time allowed before attempting to aquire the lock fails and control is returned.</param>
        /// <param name="success">True if the read lock is entered; False if the attempt to aquire the read lock timed out.</param>
        /// <remarks>
        /// Multiple threads can enter read lock concurrently.
        /// 
        /// This method blocks until the calling thread enters the lock, or timeout occurs.
        /// 
        /// If one or more threads are waiting to enter write lock, a thread that calls the <see cref="EnterReadLock"/> method 
        /// blocks until those threads have entered and then exited, or timeout occurs.
        /// </remarks>
        /// <returns>An <see cref="IDisposable"/> handle which will release the lock when disposed.</returns>
        public IDisposable TryEnterReadLock(TimeSpan timeout, out bool success)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            lock (@lock)
            {
                timer.Start();

                _waitingReaders++;

                while (_state < 0 || _waitingWriters > 0)
                {
                    TimeSpan maxWait = timeout - timer.Elapsed;
                    if (!Monitor.Wait(@lock, maxWait))
                    {
                        _waitingReaders--;
                        success = false;
                        return NullReleaser;
                    }
                }

                _state++;
                _waitingReaders--;

                timer.Stop();

                Assert.Greater(_state, 0);
                Assert.GreaterOrEqual(_waitingReaders, 0);

                success = true;
                return new Releaser(ExitReadLock);
            }
        }
        /// <summary>
        /// Attemps to enter and aquire exclusive write access. Blocks and queues for access until all preceeding locks have 
        /// exited or timeout occurs.
        /// </summary>
        /// <param name="timeout">Span of time allowed before attempting to aquire the lock fails and control is returned.</param>
        /// <param name="success">True if the write lock is entered; False if the attempt to aquire the write lock timed out.</param>
        /// <remarks>
        /// Write locks are exclusive, and no other locks can be taken while a write lock is held.
        /// 
        /// This method blocks until the calling thread enters the lock, or timeout occurs.
        /// 
        /// If other threads have entered read lock, a thread that calls the <see cref="EnterWriteLock"/> method blocks until 
        /// those threads have exited read lock, or timeout occurs. When there are threads waiting to enter write mode, 
        /// additional threads that try to enter read lock block until all the threads waiting to enter write lock have entered 
        /// and then exited, or timeout occurs.
        /// </remarks>
        /// <returns>An <see cref="IDisposable"/> handle which will release the lock when disposed.</returns>
        public IDisposable TryEnterWriteLock(TimeSpan timeout, out bool success)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            lock (@lock)
            {
                timer.Start();

                _waitingWriters++;

                while (_state != 0)
                {
                    TimeSpan maxWait = timeout - timer.Elapsed;
                    if (!Monitor.Wait(@lock))
                    {
                        _waitingWriters--;
                        success = false;
                        return NullReleaser;
                    }
                }

                _state = -1;
                _waitingWriters--;

                timer.Stop();

                Assert.GreaterOrEqual(_waitingWriters, 0);

                success = true;
                return new Releaser(ExitWriteLock);
            }
        }

        private void ExitWriteLock()
        {
            lock (@lock)
            {
                Assert.AreEqual(_state, -1);

                _state = 0;
                Monitor.PulseAll(@lock);
            }
        }

        private void ExitReadLock()
        {
            lock (@lock)
            {
                Assert.Greater(_state, 0);

                _state--;
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
