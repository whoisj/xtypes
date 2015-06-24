using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace XTypes.Tests
{
    /// <summary>
    /// Summary description for SlimReaderWriterLockFixture
    /// </summary>
    [TestClass]
    public class SlimReaderWriterLockFixture
    {
        const int Parallelism = 32;

        [TestMethod]
        public void ReaderWriterLockBasicAsync()
        {
            int maxRead = 0;
            int maxWrite = 0;
            object @lock = new object();

            SlimReadWriteLock rwl = new SlimReadWriteLock();
            Action readSub = () =>
            {
                Verify.IsFalse(rwl.EnteredWriters > 0, "Read and write locks held concurrently");

                lock (@lock)
                {
                    int readers = rwl.EnteredReaders;
                    if (readers > maxRead)
                    {
                        maxRead = readers;
                    }

                    int writers = rwl.EnteredWriters;
                    if (writers > maxWrite)
                    {
                        maxWrite = writers;
                    }
                }

                // simulate long running read operation
                Thread.Sleep(TimeSpan.FromMilliseconds(1));

                Verify.IsFalse(rwl.IsReadLockEntered && rwl.IsWriteLockEntered, "Read and write locks held concurrently");
            };
            Func<Task> reader = async () =>
            {
                using (rwl.EnterReadLock())
                {
                    await Task.Run(readSub);
                }
            };
            Action writeSub = () =>
            {
                Verify.IsFalse(rwl.EnteredReaders > 0, "Read and write locks held concurrently");

                lock (@lock)
                {
                    int readers = rwl.EnteredReaders;
                    if (readers > maxRead)
                    {
                        maxRead = readers;
                    }

                    int writers = rwl.EnteredWriters;
                    if (writers > maxWrite)
                    {
                        maxWrite = writers;
                    }
                }

                // simulate long running write operation
                Thread.Sleep(TimeSpan.FromMilliseconds(10));

                Verify.IsFalse(rwl.IsReadLockEntered && rwl.IsWriteLockEntered, "Read and write locks held concurrently");
                Verify.IsTrue(rwl.WaitingReaders > 0, "Zero waiting readers when greater than zero expected");
            };
            Func<Task> writer = async () =>
            {
                using (rwl.EnterWriteLock())
                {
                    await Task.Run(writeSub);
                }
            };

            var tasks = new Task[Parallelism];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(i == Parallelism / 2 ? writer : reader);
                Thread.Sleep(1);
            }

            Task.WaitAll(tasks);

            Verify.AreEqual(maxWrite, 1, "More than a single writer was active at a time");
            Verify.IsTrue(maxRead > 1, "Parallel reads were not encoutnered");
            Verify.AreEqual(0, rwl.EnteredReaders, "Entered readers remain");
            Verify.AreEqual(0, rwl.EnteredWriters, "Entered writers remain");
        }

        [TestMethod]
        public void ReaderWriterLockBasicSync()
        {
            int maxRead = 0;
            int maxWrite = 0;
            object @lock = new object();

            SlimReadWriteLock rwl = new SlimReadWriteLock();
            Action reader = () =>
            {
                using (rwl.EnterReadLock())
                {
                    Verify.IsFalse(rwl.EnteredWriters > 0, "Read and write locks held concurrently");

                    lock (@lock)
                    {
                        int readers = rwl.EnteredReaders;
                        if (readers > maxRead)
                        {
                            maxRead = readers;
                        }

                        int writers = rwl.EnteredWriters;
                        if (writers > maxWrite)
                        {
                            maxWrite = writers;
                        }
                    }

                    // simulate long running read operation
                    Thread.Sleep(TimeSpan.FromMilliseconds(1));

                    Verify.IsFalse(rwl.IsReadLockEntered && rwl.IsWriteLockEntered, "Read and write locks held concurrently");
                }
            };
            Action writer = () =>
            {
                using (rwl.EnterWriteLock())
                {
                    Verify.IsFalse(rwl.EnteredReaders > 0, "Read and write locks held concurrently");

                    lock (@lock)
                    {
                        int readers = rwl.EnteredReaders;
                        if (readers > maxRead)
                        {
                            maxRead = readers;
                        }

                        int writers = rwl.EnteredWriters;
                        if (writers > maxWrite)
                        {
                            maxWrite = writers;
                        }
                    }

                    // simulate long running write operation
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));

                    Verify.IsFalse(rwl.IsReadLockEntered && rwl.IsWriteLockEntered, "Read and write locks held concurrently");
                    Verify.IsTrue(rwl.WaitingReaders > 0, "Zero waiting readers when greater than zero expected");
                }
            };

            var tasks = new Task[Parallelism];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(i == Parallelism / 2 ? writer : reader);
                Thread.Sleep(1);
            }

            Task.WaitAll(tasks);

            Verify.AreEqual(maxWrite, 1, "More than a single writer was active at a time");
            Verify.IsTrue(maxRead > 1, "Parallel reads were not encoutnered");
            Verify.AreEqual(0, rwl.EnteredReaders, "Entered readers remain");
            Verify.AreEqual(0, rwl.EnteredWriters, "Entered writers remain");
        }

        [TestMethod]
        public void ReaderWriterLockExcptionalAsync()
        {
            SlimReadWriteLock rwl = new SlimReadWriteLock();
            Action readerSub = () =>
            {
                Verify.IsFalse(rwl.IsReadLockEntered && rwl.IsWriteLockEntered);

                // simulate long running read operation
                Thread.Sleep(TimeSpan.FromMilliseconds(1));

                throw new Exception();
            };
            Func<Task> reader = async () =>
            {
                try
                {
                    using (rwl.EnterReadLock())
                    {
                        await Task.Run(readerSub);
                    }
                }
                catch
                {

                }
            };
            Action writerSub = () =>
            {
                Verify.IsFalse(rwl.IsReadLockEntered && rwl.IsWriteLockEntered);

                // simulate long runngin write operation
                Thread.Sleep(TimeSpan.FromMilliseconds(10));

                throw new Exception();
            };
            Func<Task> writer = async () =>
            {
                try
                {
                    using (rwl.EnterWriteLock())
                    {
                        await Task.Run(writerSub);
                    }
                }
                catch
                {

                }
            };

            var tasks = new Task[Parallelism];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(i == Parallelism / 2 ? writer : reader);
                Thread.Sleep(1);
            }

            Task.WaitAll(tasks);

            Verify.AreEqual(0, rwl.EnteredReaders, "Entered readers remain");
            Verify.AreEqual(0, rwl.EnteredWriters, "Entered writers remain");
        }

        [TestMethod]
        public void ReaderWriterLockExcptionalSync()
        {
            SlimReadWriteLock rwl = new SlimReadWriteLock();
            Action reader = () =>
            {
                try
                {
                    using (rwl.EnterReadLock())
                    {
                        Verify.IsFalse(rwl.IsReadLockEntered && rwl.IsWriteLockEntered);

                        throw new Exception();
                    }
                }
                catch
                {

                }
            };
            Action writer = () =>
            {
                try
                {
                    using (rwl.EnterWriteLock())
                    {
                        Verify.IsFalse(rwl.IsReadLockEntered && rwl.IsWriteLockEntered);

                        throw new Exception();
                    }
                }
                catch
                {

                }
            };

            var tasks = new Task[Parallelism];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(i == Parallelism / 2 ? writer : reader);
                Thread.Sleep(1);
            }

            Task.WaitAll(tasks);

            Verify.AreEqual(0, rwl.EnteredReaders, "Entered readers remain");
            Verify.AreEqual(0, rwl.EnteredWriters, "Entered writers remain"); ;
        }
    }
}
