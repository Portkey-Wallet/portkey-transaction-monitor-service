using System;

namespace Transaction.Monitor.Common;

public class ParamException : Exception
{
    public int ErrorCode { get; }

    public ParamException() : base("Guard key error occurred.")
    {
    }

    public ParamException(string message) : base(message)
    {
    }

    public ParamException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ParamException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}