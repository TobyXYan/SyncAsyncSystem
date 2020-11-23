using CommandLib;
using CommandLib.Interfaces;
using System;

namespace SyncAsyncTest
{
    public class Component
    {

        #region Constructors
        public Component(string name)
        {
            Name = name;
            _invoker = new ActiveObject();
        }
        #endregion

        #region Types
        #endregion

        #region Fields
        private ActiveObject _invoker;
        private Action _asyncDelegate;
        #endregion

        #region Properties
        public string Name { get; private set; }
        #endregion

        #region Methods

        public ICommandProxy WriteDate()
        {
            Console.WriteLine($"Enter the function of {nameof(WriteDate)}");
            return EnqueueCommand(() => PerformWriteDate(),"WriteDate function");
        }
     
        protected ICommandProxy EnqueueCommand(SimpleCommandDelegate commandDelegate, string description)
        {
            var command = CreateCommand(commandDelegate, description);
            _invoker.Submit(command);
            return command;
        }

        protected Command CreateCommand(SimpleCommandDelegate commandDelegate, string description)
        {
            return new Command(commandDelegate, description, Name);
        }

        private ICommandResult PerformWriteDate()
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
            Console.WriteLine($"Current time is {DateTime.Now.ToString()}");
            return CommandResults.Succeeded;
        }


        public IAsyncResult WriteDataAsync()
        {
            Console.WriteLine($"Enter the function of {nameof(WriteDataAsync)}");
            _asyncDelegate = () => PerformWriteDate();
            var asyncResult = _asyncDelegate.BeginInvoke(null,null);
            return asyncResult;
        }

        public void Initialize()
        {
            _invoker.Start();
        }
        #endregion

    }
}
