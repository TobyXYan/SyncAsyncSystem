using CommandLib.Interfaces;
using System;
using System.Threading;

namespace CommandLib
{
    public delegate ICommandResult SimpleCommandDelegate();
    public delegate ICommandResult CommandDelegate(ICommand command);
   
    public class Command : ICommand
    {

        #region Constructors
        public Command()
        {
            _id = Interlocked.Increment(ref _idCounter).ToString();
            //timeStamp = TimeStamp.Now;
        }
        
        public Command(SimpleCommandDelegate simpleCommandDelegate)
            : this()
        {
            _commandDelegate = command => simpleCommandDelegate();
        }

        public Command(CommandDelegate commandDelegate)
            : this()
        {
            _commandDelegate = commandDelegate;
        }

        public Command(SimpleCommandDelegate commandDelegate, string description, string source)
            : this(commandDelegate)
        {
            ValidateDescriptionAndSource(description, source);
        }
        #endregion

        #region Types
        #endregion

        #region Fields
        private static int _idCounter;
        private readonly string _id;
        private readonly CommandDelegate _commandDelegate;
        private string _cmdSource;
        private readonly object threadLock = new object();
        private readonly EventList _commandCompletionListeners = new EventList();
        private ICommandResult _result;
        private CommandStates _state;
        private volatile bool _cancelRequested;
        #endregion

        #region Properties
        public string Description { get; private set; }
        public ICommandResult Result
        {
            get
            {
                lock (threadLock)
                {
                    return _result;
                }
            }
            set
            {
                lock (threadLock)
                {
                    _result = value;
                }
            }
        }

        public bool IsCompleted
        {
            get
            {
                switch (State)
                {
                    case CommandStates.Completed:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public virtual CommandStates State
        {
            get
            {
                lock (threadLock)
                {
                    return _state;
                }
            }
            protected set
            {
                lock (threadLock)
                {
                    CommandStates originalState = _state;
                    if (originalState == value)
                        return;

                    _state = value;

                    // Logging block
                    if (_cmdSource != null)
                    {
                        if (value != CommandStates.Completed)
                        {
                            //Log.WriteIfEnabled(StateChangeLogCategory, cmdSource, "Command {0} [{1}] changing state from {2} to {3}.", id,
                            //                         Description, originalState.ToString(), value.ToString());

                            //if (value == CommandStates.Running)
                            //    Log.WriteIfEnabled(StartedLogCategory, cmdSource, "Command {0} [{1}] started.", id, Description);
                        }
                        //else if (Result.Succeeded)
                        //    Log.WriteIfEnabled(SucceededLogCategory, cmdSource, "Command {0} [{1}] completed successfully.", id,
                        //                             Description);
                        //else
                        //    Log.WriteIfEnabled(FailedLogCategory, cmdSource, "Command {0} [{1}] failed with error code \"{2}\".", id,
                        //                             Description, ErrorCode);
                    }
                }

                // Signal outside the lock to avoid potential deadlock conditions.
                if (value == CommandStates.Completed)
                    _commandCompletionListeners.Signal();
            }
        }
        #endregion

        #region Methods
        private void ValidateDescriptionAndSource(string description, string source)
        {
            Description = description;
            _cmdSource = source;
        }

        public ICommandResult WaitForCompletion()
        {
            WaitForCompletion(Timeout.Infinite);
            return Result;//Result has been set in NoteCommandCompleted - Set state and result
        }

        public bool WaitForCompletion(TimeSpan timeLimit)
        {
            return WaitForCompletion((int)timeLimit.TotalMilliseconds);
        }

        public virtual bool WaitForCompletion(int timeLimitInMilliseconds)
        {
            ManualResetEvent commandCompletionEvent; // NOTE: event belongs to current thread, not the class instance.  This design prevents the class from having to implement IDisposable.
            lock (threadLock)
            {
                if (IsCompleted)
                    return true;
                if (timeLimitInMilliseconds == 0)
                    return false;

                commandCompletionEvent = new ManualResetEvent(false);
                _commandCompletionListeners.Add(commandCompletionEvent);
            }
            bool completed = commandCompletionEvent.WaitOne(timeLimitInMilliseconds, false);
            // using true here doens't actually exit the synchronization block.
            _commandCompletionListeners.Remove(commandCompletionEvent);
            return completed;
        }

        public virtual ICommandProxy NoteCommandCompleted()
        {
            NoteCommandCompleted(CommandResults.Succeeded);
            return this;
        }

        public virtual ICommandProxy NoteCommandCompleted(string errorCode)
        {
            NoteCommandCompleted(string.IsNullOrEmpty(errorCode) ? CommandResults.Succeeded : new CommandResult(errorCode));
            return this;
        }

        public virtual ICommandProxy NoteCommandCompleted(ICommandResult commandResult)
        {
            lock (threadLock)
            {
                if (!IsCompleted)
                {
                    Result = commandResult;
                    State = CommandStates.Completed;
                }
            }
            return this;
        }

        public void NoteCommandQueued()
        {
            State = CommandStates.Queued;
        }

        public void Invoke()
        {
            lock (threadLock)
            {
                switch ((CommandStates)_state)
                {
                    case CommandStates.Defined:
                    case CommandStates.Queued:
                        if (_cancelRequested)
                        {
                            NoteCommandCompleted(CommandResults.Cancelled);
                            return;
                        }
                        State = CommandStates.Running;
                        break;
                    case CommandStates.Running:
                        // Do nothing, already invoked.
                        return;
                    case CommandStates.Completed:
                        // Do nothing, already invoked.
                        return;
                }
            }

            if (null == _commandDelegate)
            {
                NoteCommandCompleted(CommandResults.Failed("Null command delegate"));
                return;
            }

            var commandResult = _commandDelegate(this);
            if (commandResult != CommandResults.InProgress)
                NoteCommandCompleted(commandResult);
        }

        public void Cancel()
        {
            lock (threadLock)
            {
                switch ((CommandStates)_state)
                {
                    case CommandStates.Running:
                        // Command is in progress; signal command processor to cancel.
                        _cancelRequested = true;
                        return;
                    case CommandStates.Completed:
                        // Command has already been completed.
                        return;
                    default:
                        // Command has not yet started executing; cancel.
                        NoteCommandCompleted(CommandResults.Cancelled);
                        return;
                }
            }
        }
        #endregion











        public bool IsCancelRequested => throw new NotImplementedException();

        

        public TimeSpan Age => throw new NotImplementedException();

        public IEventList CommandCompletionListeners => throw new NotImplementedException();


        public bool IsDefined => throw new NotImplementedException();

        public bool IsQueued => throw new NotImplementedException();

        public bool IsRunning => throw new NotImplementedException();

        public bool IsActive => throw new NotImplementedException();

       

        public bool Succeeded => throw new NotImplementedException();

        public bool Failed => throw new NotImplementedException();

        public string ErrorCode => throw new NotImplementedException();

        ICommandResult ICommandProxy.Result => throw new NotImplementedException();

       

        

     

      

        public void NoteCommandStarted()
        {
            throw new NotImplementedException();
        }

    }
}
