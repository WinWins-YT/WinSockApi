using System.Runtime.InteropServices;
using System.Text;
using ConsoleChat.Protocol.Enums;
using ConsoleChat.Protocol.Extensions;
using ConsoleChat.Protocol.Structs;
using WinSockApi;
using WinSockApi.Exceptions;

var clientSockets = new List<WinSock>();
var listenThreadCancel = new CancellationTokenSource();
var receiveThreadCancel = new CancellationTokenSource();
var nameDictionary = new Dictionary<long, long>();

try
{
    var socket = WinSock.Create();
    Console.WriteLine("Binding to 27015 port...");
    socket.Bind("127.0.0.1", 27015);
    Console.WriteLine("Listening...");
    socket.Listen();
    Console.WriteLine("Waiting for client connections...");
    var listenThread = new Thread(() =>
    {
        while (!listenThreadCancel.IsCancellationRequested)
        {
            var newSocket = socket.Accept();
            clientSockets.Add(newSocket);
            Console.WriteLine("Client connected");
        }
    });
    listenThread.Start();
    var receivingThread = new Thread(() =>
    {
        while (!receiveThreadCancel.IsCancellationRequested)
        {
            try
            {
                foreach (var sock in clientSockets.ToList())
                {
                    if (!sock.IsDataAvailable()) continue;
                    var buffer = sock.Receive(Marshal.SizeOf<MemeExchangeProtocol>());

                    if (buffer.Length == 0)
                    {
                        sock.Dispose();
                        clientSockets.Remove(sock);
                    }

                    Console.WriteLine($"Data received ({buffer.Length} bytes)");
                    var packet = buffer.FromByteArray<MemeExchangeProtocol>();

                    switch (packet.Command)
                    {
                        case (byte)CommandType.SendMessage:
                            var text = sock.Receive(packet.MessageLength);
                            if (!nameDictionary.ContainsKey(packet.To))
                                break;
                            
                            var toSocket = clientSockets.FirstOrDefault(x => x.Socket == nameDictionary[packet.To]);
                            if (toSocket is null)
                                break;
                            
                            var sent = toSocket.Send(buffer);
                            sent += toSocket.Send(text);
                            Console.WriteLine($"Sent message to {packet.To} ({sent} bytes)");
                            break;
                        
                        case (byte)CommandType.Register:
                            nameDictionary[packet.From] = sock.Socket;
                            Console.WriteLine($"Registered sockets {packet.From} => {sock.Socket}");
                            break;
                    }
                    //Console.WriteLine($"Data received ({buffer.Length} bytes): {Encoding.ASCII.GetString(buffer)}");
                }
            }
            catch (WinSockException) {}
        }
    });
    receivingThread.Start();
    while (true)
    {
        var command = Console.ReadLine()!;
        switch (command.ToLower())
        {
            case "exit":
                listenThreadCancel.Cancel();
                listenThread.Join(1000);
                receiveThreadCancel.Cancel();
                receivingThread.Join(1000);
                socket.Dispose();
                break;
        }
    }
}
catch (WsaException exception)
{
    Console.WriteLine(exception.GetErrorCode());
}
catch (WinSockException exception)
{
    Console.WriteLine(exception.GetErrorCode());
}