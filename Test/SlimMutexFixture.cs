using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace XTypes.Tests
{
    /// <summary>
    /// Summary description for SlimMutexFixture
    /// </summary>
    [TestClass]
    public class SlimMutexFixture
    {
        const int Parallelism = 32;

        [TestMethod]
        public void MutextBasicAsync()
        {
            int entered = 0;

            SlimMutex mutex = new SlimMutex();
            Action waiterSub = () =>
            {
                Verify.IsTrue(mutex.IsEntered);

                Interlocked.Increment(ref entered);

                Verify.AreEqual(1, entered, $"Expected 1, found {entered}");

                // simulate workload
                Thread.Sleep(TimeSpan.FromMilliseconds(1));

                Verify.AreEqual(1, entered, $"Expected 1, found {entered}");

                Interlocked.Decrement(ref entered);

                Verify.AreEqual(0, entered, $"Expected 0, found {entered}");
            };
            Func<Task> waiter = async () =>
            {
                using (mutex.Enter())
                {
                    await Task.Run(waiterSub);
                }
            };

            var tasks = new Task[Parallelism];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(waiter);
                Thread.Sleep(0);
            }

            Task.WaitAll(tasks);
        }

        [TestMethod]
        public void MutextBasicSync()
        {
            int entered = 0;

            SlimMutex mutex = new SlimMutex();
            Action waiter = () =>
            {
                using (mutex.Enter())
                {
                    Verify.IsTrue(mutex.IsEntered);

                    Interlocked.Increment(ref entered);

                    Verify.AreEqual(1, entered);

                    // simulate workload
                    Thread.Sleep(TimeSpan.FromMilliseconds(1));

                    Verify.AreEqual(1, entered, $"Expected 1, actual {entered}");

                    Interlocked.Decrement(ref entered);

                    Verify.AreEqual(0, entered, $"Expected 0, actual {entered}");
                }
            };

            var tasks = new Task[Parallelism];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(waiter);
                Thread.Sleep(0);
            }

            Task.WaitAll(tasks);
        }

        [TestMethod]
        public void MutextTimedAsync()
        {
            SlimMutex mutex = new SlimMutex();
            bool firstEntered = false;
            bool secondEntered = false;
            bool success = true;
            object signal = new object();
            Action firstSub = () =>
            {

                lock (signal)
                {
                    firstEntered = true;
                    Monitor.PulseAll(signal);

                    while (!secondEntered)
                    {
                        Monitor.Wait(signal);
                    }
                }

            };
            Func<Task> first = async () =>
            {
                using (mutex.Enter())
                {
                    await Task.Run(firstSub);
                }
            };

            Action secondSub = () =>
            {
                lock (signal)
                {
                    secondEntered = true;
                    Monitor.PulseAll(signal);
                }

            };
            Func<Task> second = async () =>
            {
                using (mutex.TryEnter(TimeSpan.FromMilliseconds(10), out success))
                {
                    await Task.Run(secondSub);
                }
            };

            Task[] tasks = new Task[2];
            tasks[0] = Task.Run(first);

            lock (signal)
            {
                while (!firstEntered)
                {
                    Monitor.Wait(signal);
                }
            }

            tasks[1] = Task.Run(second);

            Task.WaitAll(tasks);

            Verify.AreEqual(success, false);
            Verify.AreEqual(firstEntered, true);
            Verify.AreEqual(secondEntered, true);
        }

        [TestMethod]
        public void MutextTimedSync()
        {
            SlimMutex mutex = new SlimMutex();
            bool firstEntered = false;
            bool secondEntered = false;
            bool success = true;
            object signal = new object();
            Action first = () =>
            {
                using (mutex.Enter())
                {
                    lock (signal)
                    {
                        firstEntered = true;
                        Monitor.PulseAll(signal);

                        while (!secondEntered)
                        {
                            Monitor.Wait(signal);
                        }
                    }
                };
            };
            Action second = () =>
            {
                using (mutex.TryEnter(TimeSpan.FromMilliseconds(10), out success))
                {
                    lock (signal)
                    {
                        secondEntered = true;
                        Monitor.PulseAll(signal);
                    }
                }
            };

            Task[] tasks = new Task[2];
            tasks[0] = Task.Run(first);

            lock (signal)
            {
                while (!firstEntered)
                {
                    Monitor.Wait(signal);
                }
            }

            tasks[1] = Task.Run(second);

            Task.WaitAll(tasks);

            Verify.AreEqual(success, false);
            Verify.AreEqual(firstEntered, true);
            Verify.AreEqual(secondEntered, true);
        }
    }
}
