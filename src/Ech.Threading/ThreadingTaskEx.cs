using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Ech.Threading
{
    public abstract class ThreadingTaskEx : ThreadingTask
    {
        #region Private members
        private EventWaitHandle _waitHandle;
        private bool _sleeping = false;
        private bool _stopping = false;
        private object _lockObj = new object();

        private TimerCallback _timerDelegate;
        private Timer _timer;
        #endregion

        #region Constructors
        public ThreadingTaskEx(string taskName)
            :base(taskName)
        {
            _waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

            _timerDelegate = new TimerCallback(TimerStatusChecker);
            _timer = new Timer(_timerDelegate);

            base.ThreadingEvent += new ThreadingTaskEventHandler(ThreadingTaskEx_ThreadingEvent);
        }

        void ThreadingTaskEx_ThreadingEvent(object cls, ThreadingTaskEventArgs eventArgs)
        {
            switch (eventArgs.OpType)
            {
                case ThreadingOpType.Start:
                    if (_waitHandle == null)
                        _waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                    break;
            }
        }

        public ThreadingTaskEx(string taskName, bool autoStart)
            : this(taskName)
        {
            if (autoStart)
            {
                Start();
            }
        }

        public ThreadingTaskEx(string taskName, ThreadingTaskType taskType, bool autoStart)
            : this(taskName)
        {
            base.TaskType = taskType;
            if (autoStart)
            {
                Start();
            }
        }
        #endregion

        #region Public properties
        public bool IsSleeping
        {
            get { return _sleeping; }
        }

        public bool IsStopping
        {
            get { return _stopping; }
        }
        #endregion

        #region Public methods
        public virtual void Stop()
        {
            _stopping = true;

            if (_sleeping == true)
                Wake();

            base.Kill();

            _waitHandle.Close();
            _waitHandle = null;
        }

        public new void Start()
        {
            _stopping = false;
            base.Start();
        }

        public void Wake()
        {
            Debug.Assert(_waitHandle != null);

            _sleeping = false;

            // signal the waiting thread to wake it up
            _waitHandle.Set();
        }

        public void Sleep()
        {
            Sleep(0);
        }

        public void Sleep(long milliSeconds)
        {
            Debug.Assert(_waitHandle != null);

            if (milliSeconds < 0)
                throw new ArgumentOutOfRangeException("milliSeconds");

            _sleeping = true;

            // clear the signal - cause the thread to block
            _waitHandle.Reset();

            if (milliSeconds > 0)
            {
                // create timer with period milliseconds (fires just once)
                _timer.Change(milliSeconds, System.Threading.Timeout.Infinite);
            }
        }
        #endregion

        #region Overrides
        public override void OnExecuteBeforeEndTask()
        {
            // wake up the thread before ending the task
            Wake();
        }

        public override void OnExecuteTask()
        {
            if (_sleeping == true)
            {
                // this thread waits for a signal
                if (_waitHandle != null)
                    _waitHandle.WaitOne();
            }
        }
        #endregion

        #region Timer management
        private void TimerStatusChecker(object stateInfo)
        {
            // signal the waiting thread to wake it up
            if (_waitHandle != null)
                _waitHandle.Set();
        }
        #endregion
    }
}
