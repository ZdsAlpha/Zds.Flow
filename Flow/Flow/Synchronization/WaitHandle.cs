using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Flow.Synchronization
{
    public class WaitHandle
    {
        public bool Wait(int timeout)
        {
            Monitor.Enter(this);
            bool output;
            if (timeout == -1) output = Monitor.Wait(this);
            else output = Monitor.Wait(this, timeout);
            Monitor.Exit(this);
            return output;
        }
        public void Signal()
        {
            Monitor.Enter(this);
            Monitor.Pulse(this);
            Monitor.Exit(this);
        }
        public void SignalAll()
        {
            Monitor.Enter(this);
            Monitor.PulseAll(this);
            Monitor.Exit(this);
        }
        public bool TrySignal()
        {
            if (Monitor.TryEnter(this))
            {
                Monitor.Pulse(this);
                Monitor.Exit(this);
                return true;
            }
            return false;
        }
        public bool TrySignalAll()
        {
            if (Monitor.TryEnter(this))
            {
                Monitor.PulseAll(this);
                Monitor.Exit(this);
                return true;
            }
            return false;
        }
    }
}
