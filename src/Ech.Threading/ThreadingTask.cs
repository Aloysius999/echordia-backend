using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Diagnostics;

namespace Ech.Threading
{
    public enum ThreadingTaskType
    {
        RunOnce = 1,
        RunContinuous = 2,
        RunQuantum = 3

    }

    public enum ThreadingOpType
    {
        Start = 1,
        LoopEnd = 2,
        ExecuteTask = 3,
        Killed = 4,
    }

    public class ThreadingTaskEventArgs : EventArgs
    {
        public readonly ThreadingTaskType TaskType;
        public readonly ThreadingOpType OpType;

        public ThreadingTaskEventArgs(ThreadingOpType opType, ThreadingTaskType taskType)
        {
            this.TaskType = taskType;
            this.OpType = opType;
        }

        public override string ToString()
        {
            string str = OpType.ToString() + " " + TaskType.ToString();
            return str;
        }
    }

    public abstract class ThreadingTask : MarshalByRefObject, IDisposable
    {
        public delegate void ThreadingTaskEventHandler(object cls, ThreadingTaskEventArgs eventArgs);
        public event ThreadingTaskEventHandler ThreadingEvent;

        #region Private Members
        
        private ManualResetEvent _threadHandle;
        private Thread _threadObj;
        private bool _endLoop;
        private Mutex _endLoopMutex;      
        private ThreadingTaskType _taskType;
        private int _quantumSleepInterval;
        private object _state;
        private Exception _exception;
        private string _taskName;

        #endregion Private Members

        #region Constructors

        public ThreadingTask(string taskName)
        {
            _endLoop = false;
            _threadObj = null;
            _endLoopMutex = new Mutex();
            _threadHandle = new ManualResetEvent(false);

            _taskName = taskName;
            //_threadObj = new Thread(Run);
            //_threadObj.Name = taskName;
            _quantumSleepInterval = 5000;
            _exception = null;
        }

        public ThreadingTask(string taskName, bool autoStart)
            : this(taskName)
        {
            if (autoStart)
            {
                Start();
            }
        }

        public ThreadingTask(string taskName, ThreadingTaskType taskType, bool autoStart)
            : this(taskName)
        {
            _taskType = taskType;
            if (autoStart)
            {
                Start();
            }
        }

        #endregion Constructors

        #region Public Stuff

        public object State
        {
            get { return _state; }
            set { _state = value; }
        }

        public int ManagedThreadId
        {
            get { return _threadObj.ManagedThreadId; }
        }

        public ThreadingTaskType TaskType 
        {
            get { return _taskType; }
            set { _taskType = value; }
        }

        public int QuantumSleepInterval
        {
            get { return _quantumSleepInterval; }
            set { _quantumSleepInterval = value; }
        }

        public Exception ExceptionEncountered
        {
            get { return _exception; }
            set { _exception = value; }
        }

        #region Event invocation
        private void FireThreadingEvent(ThreadingOpType opType)
        {
            if (ThreadingEvent != null)
            {
                ThreadingTaskEventArgs ev = new ThreadingTaskEventArgs(opType, _taskType);
                ThreadingEvent(this, ev);
            }
        }
        #endregion

        public override int GetHashCode()
        {
            return _threadObj.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _threadObj.Equals(obj);
        }

        public Thread Thread
        {
            get { return _threadObj; }
        }

        public void EndTask()
        {
            OnExecuteBeforeEndTask();
            Kill();
        }

        public bool EndLoop
        {
             set
             {
                if (_endLoopMutex.SafeWaitHandle.IsClosed == false)
                    _endLoopMutex.WaitOne();
                _endLoop = value;
                if (_endLoopMutex.SafeWaitHandle.IsClosed == false)
                    _endLoopMutex.ReleaseMutex();
             }
             get
             {
                bool result = false;
                if (_endLoopMutex.SafeWaitHandle.IsClosed == false)
                    _endLoopMutex.WaitOne();
                result = _endLoop;
                if (_endLoopMutex.SafeWaitHandle.IsClosed == false)
                    _endLoopMutex.ReleaseMutex();
                return result;
             }
         }

         public WaitHandle Handle
         {
             get { return _threadHandle; }
         }

         public void Start()
         {
            // SKS
            // Thread value must be stored on the client thread and not in the constructor
            _threadObj = new Thread(Run);
             _threadObj.Name = _taskName;
             _endLoop = false;

             // SKS - safe handles may previously have been closed during disposal in Kill()
             if (_endLoopMutex.SafeWaitHandle.IsClosed == true)
             {
                 _endLoopMutex = new Mutex();
             }
             if (_threadHandle.SafeWaitHandle.IsClosed == true)
             {
                 _threadHandle = new ManualResetEvent(false);
             }

            Debug.Assert(_threadObj != null);
             Debug.Assert(_threadObj.IsAlive == false);
             FireThreadingEvent(ThreadingOpType.Start);
             _threadObj.Start();
         }

         public virtual void Dispose()
         {
            Kill();
         }

         private void Run()
         {
             try
             {
                 while (EndLoop == false)
                 {
                     FireThreadingEvent(ThreadingOpType.ExecuteTask);

                     switch (TaskType)
                     {
                         case ThreadingTaskType.RunOnce:
                             OnExecuteTask();
                             Thread.Sleep(0);
                             EndLoop = true;
                             break;

                         case ThreadingTaskType.RunContinuous:
                             OnExecuteTask();
                             Thread.Sleep(0);
                             break;

                         case ThreadingTaskType.RunQuantum:
                             OnExecuteTask();
                             try
                             {
                                 Thread.Sleep(QuantumSleepInterval);
                             }
                             catch (ThreadInterruptedException ex)
                             {
                             }
                             break;
                     }
                 }
             }
            catch (Exception ex)
             {
                 // SKS 7Feb15 - log any exception and throw it
                 System.Diagnostics.Trace.WriteLine("ThreadingTask.Run - _taskName=" + _taskName + " _ " + ex);
                 throw ex;
             }
             finally
             {
                 if (!_threadHandle.SafeWaitHandle.IsClosed)
                 {
                     try //AVez 22Jan2015, bug 4239, ObjectDisposedException as it may be closed just after the check in concurrent.
                     {
                        _threadHandle.Set();
                     }
                     catch { }
                 }
             }

             FireThreadingEvent(ThreadingOpType.LoopEnd);
         }

         public void Kill()
         {
            //Kill is called on client thread - must use cached thread object
            //Debug.Assert(_threadObj != null);
            //if (IsAlive == false)
            //{
            //    return;
            //}

            EndLoop = true;

            if (_threadObj != null && !_threadHandle.SafeWaitHandle.IsClosed)
            {
#if DEBUG
                _threadHandle.WaitOne(30 * 1000);    // wait for EndLoop to take effect
#else
                _threadHandle.WaitOne(10 * 1000);    // wait for EndLoop to take effect
#endif
            }

            //Wait for thread to die
            bool workerDied = Join();
             if (workerDied)
             {
                 _endLoopMutex.Close();
                 _threadHandle.Close();
             }

             FireThreadingEvent(ThreadingOpType.Killed);
         }

        public void Abort()
        {
            //Debug.Assert(_threadObj != null);
            if (IsAlive == false)
            {
                return;
            }

            EndLoop = true;

            if (!_threadHandle.SafeWaitHandle.IsClosed)
            {
#if DEBUG
                _threadHandle.WaitOne(30 * 1000);    // wait for EndLoop to take effect
#else
                _threadHandle.WaitOne(10 * 1000);    // wait for EndLoop to take effect
#endif
            }

            if (_threadObj != null)
            {
                _threadObj.Abort();
            }

            if (_endLoopMutex != null)
            {
                _endLoopMutex.Close();
            }

            if (_threadHandle != null)
            {
                _threadHandle.Close();
            }

            FireThreadingEvent(ThreadingOpType.Killed);
        }

        public bool Join()
         {
             return Join(Timeout.Infinite);
         }

         public bool Join(int millisecondsTimeout)
         {
             TimeSpan timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
             return Join(timeout);
         }

         public bool Join(TimeSpan timeout)
         {
            //Join is called on client thread - must use cached thread object
            Debug.Assert(_threadObj != null);
             if (IsAlive == false)
             {
                 return true;
             }
             Debug.Assert(Thread.CurrentThread.ManagedThreadId != _threadObj.ManagedThreadId);
             return _threadObj.Join(timeout);
         }

         // Assigns or reads the name of the underlying task
         public string Name
         {
             get { return _threadObj.Name; }
             set { _threadObj.Name = value; }
         }

         public bool IsAlive
         {
             get
             {
                //Debug.Assert(_threadObj != null);
                 if (_threadObj == null)
                     return false;

                 // SKS - safe handles may previously have been closed during disposal in Kill()
                 if (_threadHandle.SafeWaitHandle.IsClosed == false)
                 {
                     bool handleSignaled = _threadHandle.WaitOne(TimeSpan.Zero, true);
                     while (handleSignaled == _threadObj.IsAlive)
                     {
                         Thread.Sleep(0);
                     }
                 }
                 return _threadObj.IsAlive;
             }
         }


#endregion Public Stuff

#region Methods that must be provided by Derived Classes
         // dispose any state or objects you have 
        public abstract void OnExecuteBeforeEndTask();

        // execute the task
        // SALI - Note: Exception should not be thrown back and should be handled internally!
        // Exception should be stored for later purposes in Exception member
        public abstract void OnExecuteTask();

#endregion Methods that must be provided by Derived Classes

    }    
}
