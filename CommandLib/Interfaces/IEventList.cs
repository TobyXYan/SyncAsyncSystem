using System.Threading;

namespace CommandLib.Interfaces
{
    public interface IEventList
    {
		void Add(EventWaitHandle eventHandle);
		void Remove(EventWaitHandle eventHandle);
	}
}
