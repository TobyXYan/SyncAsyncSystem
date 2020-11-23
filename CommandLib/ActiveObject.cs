using CommandLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLib
{
    /// <summary>
    /// ActiveObject Design Pattern
    /// </summary>
    public class ActiveObject
    {

        #region Constructors
        public ActiveObject()
        {
            _threadEvent = new AutoResetEvent(false);//False initial state force the polling thread stucked untial one command queued
            _commandQueue = new CommandQueue<Command>(_threadEvent);
        }
        #endregion

        #region Types
        #endregion

        #region Fields
        bool _stop;
        private CommandQueue<Command> _commandQueue;
        private AutoResetEvent _threadEvent;
        private Thread _thread;
        #endregion

        #region Properties
        public EventWaitHandle WaitHandle { get; private set; }
        #endregion

        #region Methods
        public void Start()
        {
            _stop = false;
            _thread = new Thread(Run);
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void Run()
        {
            while(!_stop)
            {
                var command = _commandQueue.Dequque();
                if (command != null)
                    command.Invoke();
                _threadEvent.WaitOne(TimeSpan.FromSeconds(2),false);
            }
        }

        public void Stop()
        {
            _stop = true;
            if (_thread == null || !_thread.IsAlive)
                return;
            _threadEvent.Set();//Ask immediate polling

            if (_thread.Join(5000) == false)
                _thread.Abort();
            _thread = null;
        }

        public ICommandProxy Submit(Command command)
        {
            if (null == _commandQueue)
                return null;
            return _commandQueue.Enqueue(command);
        }
        #endregion

    }
}
