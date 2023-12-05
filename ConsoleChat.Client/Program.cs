using System.Text;
using WinSockApi;

using var socket = WinSock.Create();
socket.Connect("127.0.0.1", 27015);
Console.WriteLine("Connected");
Console.Write("Enter message: ");
var bytes = socket.Send(Encoding.ASCII.GetBytes(Console.ReadLine()!));
Console.WriteLine($"{bytes} bytes sent");