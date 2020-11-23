
namespace CommandLib.Interfaces
{
    public interface ICommand:ICommandProxy
    {
		/// <summary>
		/// Provides an interface for the command execution object to set the result.
		/// </summary>
		new ICommandResult Result { get; set; }

		/// <summary>
		/// Invokes the delegate held by the command.
		/// </summary>
		void Invoke();

		/// <summary>
		/// Query to determine if the caller has requested that the command be cancelled.
		/// </summary>
		bool IsCancelRequested { get; }

		/// <summary>
		/// Marks the command as queued.
		/// </summary>
		void NoteCommandQueued();

		/// <summary>
		/// Marks the command as started.
		/// </summary>
		/// This transition is normally effected by invoking the command using the Invoke() method.
		void NoteCommandStarted();

		/// <summary>
		/// Marks the command as completed without error code.
		/// </summary>
		/// <returns>Returns the command proxy (this)</returns>
		ICommandProxy NoteCommandCompleted();

		/// <summary>
		/// Marks the command as completed with the specified error code.
		/// </summary>
		/// <param name="errorCode">Error encountered during command execution; null and String.Empty indicate success</param>
		/// <returns>Returns the command proxy (this)</returns>
		ICommandProxy NoteCommandCompleted(string errorCode);

		/// <summary>
		/// Marks the command as completed and updates the result atomically.
		/// </summary>
		/// <param name="commandResult">Result</param>
		/// <returns>Returns the command proxy (this)</returns>
		ICommandProxy NoteCommandCompleted(ICommandResult commandResult);

		/// <summary>
		/// Description of the command.  Usually published to the UI to describe the current activity.
		/// </summary>
		string Description { get; }
	}
}
