using System.Runtime.InteropServices;

namespace ConsoleChat.Protocol.Structs;

public struct MemeExchangeProtocol
{
    public byte Command;
    public long From;
    public long To;
    public int MessageLength;
}