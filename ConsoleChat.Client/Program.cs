using System.Runtime.InteropServices;
using System.Text;
using ConsoleChat.Protocol.Enums;
using ConsoleChat.Protocol.Extensions;
using ConsoleChat.Protocol.Structs;
using WinSockApi;
using WinSockApi.Exceptions;

using var socket = WinSock.Create();
Console.WriteLine("Welcome to console chat");
Console.Write("Enter server host address: ");
var address = Console.ReadLine()!.Split(":");
var host = address[0];
var port = address.Length == 2 ? ushort.Parse(address[1]) : (ushort)27015;
socket.Connect(host, port);
var exiting = false;
var listeningThread = new Thread(ProcessMessages);
listeningThread.Start();
Console.WriteLine("Connected");
Console.WriteLine("Registering...");
var regPacket = new MemeExchangeProtocol
{
    Command = (byte)CommandType.Register,
    From = socket.Socket
};
Console.WriteLine($"Sent registration packet ({socket.Send(regPacket.ToByteArray())} bytes)");

Console.WriteLine("Command mode (type \"help\" for help)");

while (!exiting)
{
    try
    {
        Console.Write(">");
        var cmd = Console.ReadLine()!;

        switch (cmd.Split(' ')[0].ToLower())
        {
            case "help":
                Console.WriteLine("send %1 %2 - send message to ID %1 with text %2");
                Console.WriteLine("whoami - view yourself ID");
                Console.WriteLine("exit - close application");
                break;

            case "whoami":
                Console.WriteLine($"Your ID: {socket.Socket}");
                break;
            
            case "exit":
                exiting = true;
                break;

            case "send":
                var msgArr = cmd.Split(' ');
                if (msgArr.Length < 2)
                {
                    Console.WriteLine("Wrong send usage. Type \"help\" for help.");
                    break;
                }

                var to = msgArr[1];
                var msg = string.Join(" ", msgArr[2..]);
                var msgBytes = Encoding.UTF8.GetBytes(msg);

                var packet = new MemeExchangeProtocol
                {
                    Command = (byte)CommandType.SendMessage,
                    From = socket.Socket,
                    To = int.Parse(to),
                    MessageLength = msgBytes.Length
                };

                var sentBytes = socket.Send(packet.ToByteArray());

                sentBytes += socket.Send(msgBytes);
                
                Console.WriteLine($"Sent {sentBytes} bytes");
                
                break;
            
            default:
                Console.WriteLine("Unknown command. Type \"help\" for help.");
                break;
        }
    }
    catch (WinSockException ex)
    {
        Console.WriteLine("Socket exception: " + ex);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Something wrong: {ex}");
    }
}

void ProcessMessages()
{
    while (!exiting)
    {
        if (socket.IsDataAvailable())
        {
            var buffer = socket.Receive(Marshal.SizeOf<MemeExchangeProtocol>());

            var packet = buffer.FromByteArray<MemeExchangeProtocol>();

            if (packet.To == socket.Socket)
            {
                var text = socket.Receive(packet.MessageLength);
                
                Console.WriteLine();
                Console.WriteLine($"Message from {packet.From}");
                Console.WriteLine(Encoding.UTF8.GetString(text));
                Console.Write(">");
            }
        }
        
        Thread.Sleep(500);
    }
}