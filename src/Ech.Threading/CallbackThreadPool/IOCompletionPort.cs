using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace Ech.Threading.CallbackThreadPool
{
    internal sealed class IOCompletionPort : IDisposable
    {
        private static readonly IntPtr c_InvalidHandleValue = new IntPtr(-1);
        private IntPtr m_hIOCP;

        public IOCompletionPort(int maxConcurrency)
        {
            this.m_hIOCP = CreateIoCompletionPort(c_InvalidHandleValue, IntPtr.Zero, UIntPtr.Zero, (uint) maxConcurrency);
            GC.KeepAlive(this);
        }

        [DllImport("Kernel32", SetLastError=true, ExactSpelling=true)]
        private static extern bool CloseHandle(IntPtr h);
        [DllImport("Kernel32", SetLastError=true, ExactSpelling=true)]
        private static extern IntPtr CreateIoCompletionPort(IntPtr hFile, IntPtr hExistingIOCP, UIntPtr CompKey, uint NumConcurrentThreads);
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && (this.m_hIOCP != IntPtr.Zero))
            {
                CloseHandle(this.m_hIOCP);
                this.m_hIOCP = IntPtr.Zero;
            }
            GC.KeepAlive(this);
        }

        ~IOCompletionPort()
        {
            this.Dispose(false);
        }

        [DllImport("Kernel32", SetLastError = true, ExactSpelling = true)]
        private static extern bool GetQueuedCompletionStatus(IntPtr hIOCP, out uint numBytesTransferred, out IntPtr cb, out IntPtr state, uint milliseconds);
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public void GetStatus(int millseconds, out bool timedOut, out WaitCallback cb, out object state)
        {
            cb = null;
            state = null;
            uint num;
            IntPtr value;
            IntPtr intPtr;
            bool queuedCompletionStatus = IOCompletionPort.GetQueuedCompletionStatus(this.m_hIOCP, out num, out value, out intPtr, (uint)millseconds);
            //int lastWin32Error = Marshal.GetLastWin32Error();
            timedOut = (!queuedCompletionStatus && intPtr == IntPtr.Zero);
            if (timedOut)
            {
                return;
            }

            GCHandle gCHandle = (GCHandle)value;
            cb = (WaitCallback)gCHandle.Target;
            gCHandle.Free();
            if (intPtr != IntPtr.Zero)
            {
                GCHandle gCHandle2 = (GCHandle)intPtr;
                state = gCHandle2.Target;
                gCHandle2.Free();
            }
            GC.KeepAlive(this);
        }
        //public void GetStatus(int millseconds, out bool timedOut, out WaitCallback cb, out object state)
        //{
        //    uint num;
        //    IntPtr ptr;
        //    IntPtr ptr2;
        //    cb = null;
        //    state = null;
        //    bool flag = GetQueuedCompletionStatus(this.m_hIOCP, out num, out ptr, out ptr2, (uint) millseconds);
        //    int num2 = Marshal.GetLastWin32Error();
        //    timedOut = (!flag && (ptr2 == IntPtr.Zero)) && (num2 == 0x102L);
        //    if (!timedOut)
        //    {
        //        GCHandle handle = (GCHandle) ptr;
        //        cb = (WaitCallback) handle.Target;
        //        handle.Free();
        //        if (ptr2 != IntPtr.Zero)
        //        {
        //            GCHandle handle2 = (GCHandle) ptr2;
        //            state = handle2.Target;
        //            handle2.Free();
        //        }
        //        GC.KeepAlive(this);
        //    }
        //}

        [DllImport("Kernel32", SetLastError=true, ExactSpelling=true)]
        private static extern bool PostQueuedCompletionStatus(IntPtr hIOCP, uint numBytesTransferred, IntPtr cb, IntPtr state);
        public void PostStatus(WaitCallback callback)
        {
            this.PostStatus(callback, null);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public void PostStatus(WaitCallback callback, object state)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("cb", "delegate must not be null");
            }
            GCHandle handle = GCHandle.Alloc(callback);
            GCHandle handle2 = new GCHandle();
            if (state != null)
            {
                handle2 = GCHandle.Alloc(state);
            }
            if (!PostQueuedCompletionStatus(this.m_hIOCP, 0, (IntPtr) handle, (state == null) ? IntPtr.Zero : ((IntPtr) handle2)))
            {
                handle.Free();
                if (state != null)
                {
                    handle2.Free();
                }
                throw new Exception("Failed to post callback");
            }
            GC.KeepAlive(this);
        }
    }
}

