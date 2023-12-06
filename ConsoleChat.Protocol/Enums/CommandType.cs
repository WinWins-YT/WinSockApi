namespace ConsoleChat.Protocol.Enums;

public enum CommandType : byte
{
    SendMessage = 0,
    GetMessage = 1,
    Register = 2,
}