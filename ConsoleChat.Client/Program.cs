using WinSockApi;

using var socket = WinSock.Create();
socket.Connect("127.0.0.1", 27015);
var bytes = socket.Send("Hello, world"u8.ToArray());
Console.WriteLine($"{bytes} sent");