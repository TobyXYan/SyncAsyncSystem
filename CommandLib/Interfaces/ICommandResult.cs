using System;

namespace CommandLib.Interfaces
{
    public class ICommandResult
    {
        bool Succeeded { get; }

        bool Failed { get; }

        string ErrorCode { get; }

    }
}
