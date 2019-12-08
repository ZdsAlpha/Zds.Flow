using Flow.Collections;
using Flow.Updatables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Flow.Updaters
{
    public class UpdaterX : Updater
    {
        private SafeList<Thread> threads = new SafeList<Thread>();
        private int totalThreads = 0;
        private int idleThreads = 0;
        public override event ThreadEventDelegate ThreadCreated;
        public override event ThreadEventDelegate ThreadDestroyed;
        public int TotalThreads { get => totalThreads; }
        public int IdleThreads { get => idleThreads; }
        public int SleepTime { get; set; } = 1;
        public int MaxThreads { get; set; } = 50;
        public int MaxIdleThreads { get; set; } = 10;
        public TimeSpan MinThreadLifeSpan { get; set; } = TimeSpan.FromSeconds(10);
        
        public void CreateThread()
        {
            if (!IsRunning || Targets.Length == 0) return;
            var thread = new Thread(Work);
            thread.Start();
        }
        private void Work()
        {
            if (!IsRunning || Targets.Length == 0) return;
            var thread = Thread.CurrentThread;
            threads.Add(thread);
            var _totalThreads = Interlocked.Increment(ref totalThreads);
            if (_totalThreads > MaxThreads)
            {
                Interlocked.Decrement(ref totalThreads);
                return;
            }
            Interlocked.Increment(ref idleThreads);
            ThreadCreated?.Invoke(this, thread);
            var Stopwatch = new Stopwatch.Stopwatch();
            Stopwatch.Start();
            int _idleThreads = 0;
            while (IsRunning && Targets.Length > 0)
            {
                _idleThreads = Interlocked.Decrement(ref idleThreads);
                if (idleThreads == 0 && totalThreads < MaxThreads) CreateThread();
                Update();
                _idleThreads = Interlocked.Increment(ref idleThreads);
                if (_idleThreads >= MaxIdleThreads && Stopwatch.Elasped >= MinThreadLifeSpan) break;
                if (!IsRunning || Targets.Length == 0) break;
                Delay();
            }
            Stopwatch.Stop();
            Interlocked.Decrement(ref idleThreads);
            Interlocked.Decrement(ref totalThreads);
            threads.Remove(thread);
            ThreadDestroyed?.Invoke(this, thread);
        }
        protected virtual void Delay() => Thread.Sleep(SleepTime);

        public override void Add(IUpdatable obj)
        {
            base.Add(obj);
            if (IsRunning && totalThreads == 0) CreateThread();
        }
        public override void Start()
        {
            base.Start();
            if (totalThreads == 0) CreateThread();
        }
        public override void Reset()
        {
            Stop();
            Wait();
        }

        public override bool Wait(TimeSpan timeout)
        {
            Stopwatch.Stopwatch stopwatch = new Stopwatch.Stopwatch();
            stopwatch.Start();
            while (totalThreads != 0)
            {
                Thread.Sleep(1);
                if (totalThreads == 0) break;
                if (stopwatch.Elasped >= timeout)
                {
                    stopwatch.Stop();
                    return false;
                }
            }
            stopwatch.Stop();
            return true;
        }

        public delegate void ThreadEvent(object sender, Thread thread);
    }
}
