using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Ech.Threading.CallbackThreadPool
{
    public delegate void ThreadTerminationCallback();

    public class HighWaterMarkThreadPool : IDisposable
    {
        private const uint c_WAIT_TIMEOUT = 0x102;
        private int m_idleThreadTimeout;
        private IOCompletionPort m_iocp;
        private int m_maxThreadsAllowed;
        private int m_maxThreadsEverInPool;
        private int m_numBusyThreads;
        private int m_numItemsPosted;
        private int m_numItemsProcessed;
        private int m_numThreadsInPool;
        private int m_maxPendingAllowed = 50;
        private AutoResetEvent _signalEventHandle = new AutoResetEvent(false);
        private ThreadTerminationCallback _threadTerminationCallback = null;

        public HighWaterMarkThreadPool(int idleThreadTimeout) : this(idleThreadTimeout, 10, 0)
        {
        }

        public HighWaterMarkThreadPool(int idleThreadTimeout, int maxThreadsAllowed) : this(idleThreadTimeout, maxThreadsAllowed, 0)
        {
        }

        public HighWaterMarkThreadPool(int idleThreadTimeout, int maxThreadsAllowed, int maxConcurrency)
        {
            this.m_idleThreadTimeout = 0x6ddd00;
            this.m_maxThreadsAllowed = maxThreadsAllowed;
            this.m_idleThreadTimeout = idleThreadTimeout;
            this.m_iocp = new IOCompletionPort(maxConcurrency);
        }

        public HighWaterMarkThreadPool(int idleThreadTimeout, ThreadTerminationCallback threadTerminationCallback)
            : this(idleThreadTimeout, 10, 0, threadTerminationCallback)
        {
        }

        public HighWaterMarkThreadPool(int idleThreadTimeout, int maxThreadsAllowed, ThreadTerminationCallback threadTerminationCallback)
            : this(idleThreadTimeout, maxThreadsAllowed, 0, threadTerminationCallback)
        {
        }

        public HighWaterMarkThreadPool(int idleThreadTimeout, int maxThreadsAllowed, int maxConcurrency, ThreadTerminationCallback threadTerminationCallback)
            :this(idleThreadTimeout, maxThreadsAllowed, maxConcurrency)
        {
            _threadTerminationCallback = threadTerminationCallback;
        }

        private void AddThreadToPool()
        {
            Interlocked.Increment(ref this.m_numThreadsInPool);
            InterlockedMax(ref this.m_maxThreadsEverInPool, this.m_numThreadsInPool);
            Interlocked.Increment(ref this.m_numBusyThreads);
            Thread thread = new Thread(new ThreadStart(this.ThreadPoolFunc));
            thread.IsBackground = true;
            thread.Start();
            thread.Priority = ThreadPriority.Normal;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.m_iocp.Dispose();
            }
        }

        ~HighWaterMarkThreadPool()
        {
            this.Dispose(false);
        }

        protected long IncreaseMaximumThreadsInPool(int increment)
        {
            return (long) Interlocked.Exchange(ref this.m_maxThreadsAllowed, increment);
        }

        private static int InterlockedMax(ref int target, int val)
        {
            int num;
            int num2 = target;
            do
            {
                num = num2;
                num2 = Interlocked.CompareExchange(ref target, Math.Max(num, val), num);
            }
            while (num != num2);
            return num2;
        }

        [DllImport("NTDLL")]
        private static extern long QueryInformationThread(IntPtr hThread, int ThreadInfoClass, out IntPtr fIsIoPending, uint len, out uint uRetLen);
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public void QueueUserWorkItem(WaitCallback callback)
        {
            this.QueueUserWorkItem(callback, null);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public virtual void QueueUserWorkItem(WaitCallback callback, object state)
        {
            if (this.m_numThreadsInPool == 0)
            {
                this.AddThreadToPool();
            }

            Interlocked.Increment(ref this.m_numItemsPosted);
            this.m_iocp.PostStatus(callback, state);

            if (NumItemsPending >= MaxPendingAllowed)
            {
                _signalEventHandle.WaitOne();
            }
        }

        private bool ThreadHasIoPending(Thread t)
        {
            IntPtr ptr;
            uint num;
            QueryInformationThread(IntPtr.Zero, 0x10, out ptr, 4, out num);
            return (ptr != IntPtr.Zero);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        protected void ThreadPoolFunc()
        {
            try
            {
                bool flag = true;
                while (flag)
                {
                    WaitCallback callback;
                    object obj2;
                    bool flag2;
                    Interlocked.Decrement(ref this.m_numBusyThreads);
                    this.m_iocp.GetStatus(this.m_idleThreadTimeout, out flag2, out callback, out obj2);
                    int num = Interlocked.Increment(ref this.m_numBusyThreads);
                    if (flag2)
                    {
                        flag = false;
                        return;
                    }
                    if ((num == this.m_numThreadsInPool) && (num < this.m_maxThreadsAllowed))
                    {
                        this.AddThreadToPool();
                    }
                    Interlocked.Increment(ref this.m_numItemsProcessed);
                    callback(obj2);
                    _signalEventHandle.Set();
                }
            }
            finally
            {
                Interlocked.Decrement(ref this.m_numBusyThreads);
                Interlocked.Decrement(ref this.m_numThreadsInPool);

                if (_threadTerminationCallback != null)
                    _threadTerminationCallback();
            }
        }

        public int BusyThreads
        {
            get
            {
                return this.m_numBusyThreads;
            }
        }

        public int MaxPendingAllowed
        {
            get
            {
                return this.m_maxPendingAllowed;
            }
            set
            {
                this.m_maxPendingAllowed = value;
            }
        }

        public int MaxThreadsAllowed
        {
            get
            {
                return this.m_maxThreadsAllowed;
            }
            set
            {
                Interlocked.Exchange(ref this.m_maxThreadsAllowed, value);
            }
        }

        public int MaxThreadsEverInPool
        {
            get
            {
                return this.m_maxThreadsEverInPool;
            }
        }

        public int NumItemsPending
        {
            get
            {
                return (this.NumItemsPosted - this.NumItemsProcessed);
            }
        }

        public int NumItemsPosted
        {
            get
            {
                return this.m_numItemsPosted;
            }
        }

        public int NumItemsProcessed
        {
            get
            {
                return this.m_numItemsProcessed;
            }
        }

        public int ThreadsInPool
        {
            get
            {
                return this.m_numThreadsInPool;
            }
        }
    }
}

