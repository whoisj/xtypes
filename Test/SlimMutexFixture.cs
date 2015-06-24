using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Verify = Xunit.Assert;

namespace XTypes.Tests
{
    /// <summary>
    /// Summary description for SlimMutexFixture
    /// </summary>
    public class SlimMutexFixture
    {
        const int Parallelism = 32;

        [Fact]
        public void MutextBasicAsync()
        {
            int entered = 0;

            SlimMutex mutex = new SlimMutex();
            Action waiterSub = () =>
            {
                Verify.True(mutex.IsEntered, "The mutex does not report being entered.");

                Interlocked.Increment(ref entered);

                Verify.Equal(1, entered);

                // simulate workload
                Thread.Sleep(TimeSpan.FromMilliseconds(1));

                Verify.Equal(1, entered);

                Interlocked.Decrement(ref entered);

                Verify.Equal(0, entered);
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

        [Fact]
        public void MutextBasicSync()
        {
            int entered = 0;

            SlimMutex mutex = new SlimMutex();
            Action waiter = () =>
            {
                using (mutex.Enter())
                {
                    Verify.True(mutex.IsEntered, "The mutex does not report being entered.");

                    Interlocked.Increment(ref entered);

                    Verify.Equal(1, entered);

                    // simulate workload
                    Thread.Sleep(TimeSpan.FromMilliseconds(1));

                    Verify.Equal(1, entered);

                    Interlocked.Decrement(ref entered);

                    Verify.Equal(0, entered);
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

        [Fact]
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

            Verify.False(success, "Mutex was double entered.");
            Verify.True(firstEntered, "Mutex wasn't entered the first time.");
            Verify.True(secondEntered, "Mutext wasn't entered the second time");
        }

        [Fact]
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

            Verify.False(success, "Mutex was double entered.");
            Verify.True(firstEntered, "Mutex wasn't entered the first time.");
            Verify.True(secondEntered, "Mutext wasn't entered the second time");
        }
    }
}
