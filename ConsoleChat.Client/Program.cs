using System.Text;
using WinSockApi;

using var socket = WinSock.Create();
socket.Connect("127.0.0.1", 27015);
var bytes = socket.Send(Encoding.ASCII.GetBytes("Hello, world"));
Console.WriteLine($"{bytes} bytes sent");