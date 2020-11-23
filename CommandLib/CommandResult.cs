using CommandLib.Interfaces;
using System;

namespace CommandLib
{
    public class CommandResult : ICommandResult
    {

        #region Constructors
        public CommandResult(string errorCode)
        {
            ErrorCode = (errorCode ?? string.Empty);
        }
        #endregion

        #region Types
        #endregion

        #region Fields
        #endregion

        #region Properties
        public bool Succeeded { get => string.IsNullOrEmpty(ErrorCode); }
        bool Failed { get => (!Succeeded); }
        public string ErrorCode { get; private set; }
        #endregion

        #region Methods
        #endregion

    }

    public static class CommandResults
    {
        public const ICommandResult InProgress = null;

        public const string CommandCancelledCompletionCode = "Cancelled";

        public static readonly ICommandResult Cancelled = new CommandResult(CommandCancelledCompletionCode);

        public static readonly ICommandResult Succeeded = new CommandResult(string.Empty);

        public static ICommandResult Failed(string errorCode)
        {
            if (string.IsNullOrEmpty(errorCode))
                errorCode = "Failed";
            return new CommandResult(errorCode);
        }

        public static ICommandResult Failed(string format, params object[] args)
        {
            try
            {
                return new CommandResult(String.Format(format, args));
            }
            catch (Exception e)
            {
                string errorCode = (format == null
                                            ? "Invalid format"
                                            : "Failure code with format \"" + format + "\" threw exception \"" + e + "\"");
                return new CommandResult(errorCode);
            }
        }

    }

}
