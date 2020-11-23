using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncAsyncTest
{
    class Program
    {
        /// <summary>
        /// The key point of this pattern is Return the command while put the command in another thread
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var component = new Component("Pattern test");
            component.Initialize();//Start to polling
            var asyncRet = component.WriteDataAsync();//Async way
            component.WriteDate().WaitForCompletion();//The usage of pattern.  WaitForCompletion can be put after some operations to make it like a Async way
            asyncRet.AsyncWaitHandle.WaitOne();
            //asyncRet.IsCompleted;
            Console.ReadLine();

        }
    }
}
