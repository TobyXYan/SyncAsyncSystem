using CommandLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CommandLib
{
    public class EventList : IEventList
    {
        private List<EventWaitHandle> _handleList;
        private object _handlesLock = new object();

        public void Add(EventWaitHandle eventHandle)
        {
            lock(_handlesLock)
            {
                if(_handleList == null)
                {
                    _handleList = new List<EventWaitHandle>();
                }
                if (_handleList.Contains(eventHandle))
                    return;
                _handleList.Add(eventHandle);
            }
        }

        public void Remove(EventWaitHandle eventHandle)
        {
            if (null == _handleList)
                return;
            lock(_handlesLock)
            {
                if (_handleList.Contains(eventHandle))
                    _handleList.Remove(eventHandle);
            }
        }

        public void Clear()
        {
            if (null == _handleList)
                return;
            lock(_handlesLock)
            {
                _handleList.Clear();
            }
        }

        public void Signal()
        {
            if (null == _handleList || _handleList.Count == 0)
                return;
            lock(_handlesLock)
            {
                foreach (var handle in _handleList)
                    handle.Set();
            }
        }

        public int Count
        {
            get{
                if (null == _handleList)
                    return 0;
                return _handleList.Count;
            }
        }
    }
}
