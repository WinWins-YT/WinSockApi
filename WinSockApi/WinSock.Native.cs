using System.Runtime.InteropServices;
using WinSockApi.Structs;

namespace WinSockApi;

internal static class WinSockNative
{
    [DllImport("Ws2_32.dll", EntryPoint = "socket", CallingConvention = CallingConvention.StdCall)]
    internal static extern long Socket(int af, int type, int protocol);

    [DllImport("Ws2_32.dll", EntryPoint = "closesocket")]
    internal static extern int CloseSocket(long s);

    [DllImport("Ws2_32.dll", EntryPoint = "bind")]
    internal static extern int Bind(long socket, ref SocketInputAddress name, int namelen);

    [DllImport("Ws2_32.dll")]
    internal static extern int listen(long s, int backlog);

    [DllImport("Ws2_32.dll")]
    internal static extern long accept(long s, ref SocketInputAddress addr, ref int addrlen);

    [DllImport("Ws2_32.dll")]
    internal static extern int connect(long s, ref SocketAddress name, int namelen);

    [DllImport("Ws2_32.dll")]
    internal static extern int send(long s, ref byte[] buf, int len, int flags);
}

internal static class WinSockNativeWsa
{
    [DllImport("Ws2_32.dll")]
    internal static extern int WSAStartup(ushort wVersionRequired, ref WsaData lpWSAData);
    
    [DllImport("Ws2_32.dll")]
    internal static extern int WSACleanup();
    
    [DllImport("Ws2_32.dll")]
    internal static extern int WSAGetLastError();
}

internal static class WinSockNativeTools
{
    [DllImport("Ws2_32.dll", EntryPoint = "inet_addr")]
    internal static extern ulong InetAddr([MarshalAs(UnmanagedType.LPWStr)] string cp);

    [DllImport("Ws2_32.dll", EntryPoint = "htons")]
    internal static extern ushort HostToTcp(ushort hostshort);

    [DllImport("Ws2_32.dll", EntryPoint = "getaddrinfo", CharSet = CharSet.Ansi)]
    internal static extern int GetAddressInfo([MarshalAs(UnmanagedType.LPStr)] string pNodeName,
        [MarshalAs(UnmanagedType.LPStr)] string pServiceName, ref AddressInfo pHints, ref IntPtr ppResult);
}