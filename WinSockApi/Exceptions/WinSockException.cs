using System.Runtime.Serialization;
using WinSockApi.Enums;

namespace WinSockApi.Exceptions;

public class WinSockException : Exception
{
    public int ErrorCode { get; private set; }
    
    public WinSockException()
    {
    }

    protected WinSockException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public WinSockException(string? message) : base(message)
    {
    }
    
    public WinSockException(string? message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public WinSockException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public WsaErrorCode GetErrorCode() => (WsaErrorCode)ErrorCode;

    public override string ToString()
    {
        var text = base.ToString();
        text = "Error code: " + GetErrorCode() + "\n" + text;
        return text;
    }
}