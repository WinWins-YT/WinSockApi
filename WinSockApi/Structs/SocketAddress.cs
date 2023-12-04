using System.Runtime.InteropServices;

namespace WinSockApi.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct SocketAddress
{
    internal ushort sa_family;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
    internal byte[] sa_data;
}