using WinSockApi.Enums;

namespace WinSockApi.Exceptions;

public class WsaException : WinSockException
{
    public WsaException(string? message, int errorCode) : base(message, errorCode)
    {
    }
}