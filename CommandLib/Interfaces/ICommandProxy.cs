using System;


namespace CommandLib.Interfaces
{

	public enum CommandStates
	{
		/// <summary>
		/// Pre-creation
		/// </summary>
		Undefined = 0,
		/// <summary>
		/// Defined, command has been created
		/// </summary>
		Defined,
		/// <summary>
		/// Queued, the command has been placed into a component command queue
		/// </summary>
		Queued,
		/// <summary>
		/// Running, the command is being executed by the component
		/// </summary>
		Running,
		/// <summary>
		/// Completed, the command has been completed
		/// </summary>
		Completed
	}

	public interface ICommandProxy
    {
		TimeSpan Age { get; }

		void Cancel();

		ICommandResult WaitForCompletion();

		bool WaitForCompletion(TimeSpan timeLimit);

		IEventList CommandCompletionListeners { get; }

		//Common.Data.TimeStampedData<CommandStates> TimeStampedState { get; }

		/// <summary>
		/// Provides the current command state.
		/// </summary>
		CommandStates State { get; }

		/// <summary>
		/// Query to determine whether the command is valid.
		/// </summary>
		bool IsDefined { get; }

		/// <summary>
		/// Query to determine whether the command is enqueued awaiting execution.
		/// </summary>
		bool IsQueued { get; }

		/// <summary>
		/// Query to determine whether the command is executing.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Query to determine whether the command is pending completion (enqueued or executing).
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Query to determine whether the command has completed.
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		/// Returns the command result.
		/// </summary>
		ICommandResult Result { get; }

		/// <summary>
		/// Query to determine whether the command completed successfully.  Returns false if command has not completed.
		/// </summary>
		bool Succeeded { get; }

		/// <summary>
		/// Query to determine whether the command completed with a failure.  Returns false if command has not completed.
		/// </summary>
		bool Failed { get; }

		/// <summary>
		/// Error code for the command.  String.Empty indicates success.  Returns null if command has not completed.
		/// </summary>
		string ErrorCode { get; }

	}
}
