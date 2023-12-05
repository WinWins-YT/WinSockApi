using System.Text;
using WinSockApi;
using WinSockApi.Exceptions;

var clientSockets = new List<WinSock>();
var listenThreadCancel = new CancellationTokenSource();
var receiveThreadCancel = new CancellationTokenSource();

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
            clientSockets.Add(socket.Accept());
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
                foreach (var sock in clientSockets)
                {
                    if (!sock.IsDataAvailable()) continue;
                    var buffer = sock.Receive();
                    Console.WriteLine($"Data received ({buffer.Length} bytes): {Encoding.ASCII.GetString(buffer)}");
                }
            }
            catch (InvalidOperationException) {}
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