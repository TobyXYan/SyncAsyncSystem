using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommandLib
{
    public class CommandQueue<T> where T : Command
    {

        #region Constructors
        public CommandQueue(EventWaitHandle waitHandle)
        {
            _eventWaitHandle = waitHandle;
        }
        #endregion

        #region Types
        #endregion

        #region Fields
        private EventWaitHandle _eventWaitHandle;
        private Queue<T> _queue = new Queue<T>();
        private readonly object queueLock = new object();
        #endregion

        #region Properties
        #endregion

        #region Methods
        //Here is the most critical part, put the command to the polling queue in another thread, and
        //at the same time return the command back, we can use the command returen to do sync or async operations
        public T Enqueue(T command)
        {
            if (_queue == null)
                return null;
            lock(queueLock)
            {
                _queue.Enqueue(command);
                _eventWaitHandle?.Set();//Notice Active Object to get to the next polling of command immediately
            }
            return command;//
        }

        public T Dequque()
        {
            if (null == _queue)
                return null;
            lock(queueLock)
            {
                return _queue.Count>0?_queue.Dequeue():null;
            }
            
        }
        #endregion

    }
}
