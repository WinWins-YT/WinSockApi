using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinSockApi.Enums;
using WinSockApi.Exceptions;
using WinSockApi.Structs;

namespace WinSockApi;

public class WinSock : IDisposable
{
    private readonly AddressFamily _addressFamily;
    private readonly SocketType _socketType;
    private readonly Protocol _protocol;
    private readonly long _socket;
    
    public long Socket => _socket;

    private WinSock(AddressFamily addressFamily, SocketType socketType, Protocol protocol)
    {
        _addressFamily = addressFamily;
        _socketType = socketType;
        _protocol = protocol;
        var wsaData = new WsaData();
        var status = WinSockNativeWsa.WSAStartup(0x0202, ref wsaData);
        if (status != 0)
            throw new WsaException("WSAStartup failed", status);
        
        _socket = WinSockNative.Socket((int)addressFamily, (int)socketType, (int)protocol);
        
        if (_socket == -1)
            throw new WinSockException("Socket is invalid", WinSockNativeWsa.WSAGetLastError());
    }

    private WinSock(long socket)
    {
        _socket = socket;
    }

    public static WinSock Create(AddressFamily addressFamily = AddressFamily.IpV4,
        SocketType socketType = SocketType.Stream,
        Protocol protocol = Protocol.Tcp)
    {
        return new WinSock(addressFamily, socketType, protocol);
    }

    public void Bind(string address, ushort port)
    {
        var sa = new SocketInputAddress
        {
            sin_family = (ushort)_addressFamily,
            sin_port = WinSockNativeTools.HostToTcp(port)
        };
        sa.sin_addr.S_un.S_addr = WinSockNativeTools.InetAddr(address);
        
        var status = WinSockNative.Bind(_socket, ref sa, Marshal.SizeOf(sa));
        if (status == -1)
            throw new WinSockException("Bind failed", WinSockNativeWsa.WSAGetLastError());
    }

    public void Connect(string address, ushort port)
    {
        var hints = new AddressInfo
        {
            ai_family = (short)AddressFamily.IpV4,
            ai_socktype = (short)_socketType,
            ai_protocol = (short)_protocol
        };

        var result = new AddressInfo();
        
        var resPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(result));
        Marshal.StructureToPtr(result, resPtr, false);

        var status = WinSockNativeTools.GetAddressInfo(address, port.ToString(), ref hints, ref resPtr);
        if (status != 0)
        {
            Marshal.FreeCoTaskMem(resPtr);
            throw new WinSockException("getaddrinfo failed", status);
        }

        var res = Marshal.PtrToStructure<AddressInfo>(resPtr);
        var addrInfo = Marshal.PtrToStructure<SocketAddress>(res.ai_addr);
        status = WinSockNative.connect(_socket, ref addrInfo, (int)res.ai_addrlen);
        if (status == -1)
        {
            Marshal.FreeCoTaskMem(resPtr);
            throw new WinSockException("Connect failed", WinSockNativeWsa.WSAGetLastError());
        }
        
        Marshal.FreeCoTaskMem(resPtr);
    }

    public void Listen(int maxConnections = 0x7fffffff)
    {
        var status = WinSockNative.listen(_socket, maxConnections);
        if (status == -1)
            throw new WinSockException("Listen failed", WinSockNativeWsa.WSAGetLastError());
    }

    public WinSock Accept()
    {
        var sa = new SocketInputAddress();
        var saLength = Marshal.SizeOf(sa);
        var newSocket = WinSockNative.accept(_socket, ref sa, ref saLength);
        if (newSocket == -1)
            throw new WinSockException("Accept failed", WinSockNativeWsa.WSAGetLastError());

        return new WinSock(newSocket);
    }

    public Task<WinSock> AcceptAsync()
    {
        return Task.Run(Accept);
    }

    public int Send(byte[] data)
    {
        var status = WinSockNative.send(_socket, data, data.Length, 0);
        if (status == -1)
            throw new WinSockException("Send failed", WinSockNativeWsa.WSAGetLastError());

        return status;
    }

    public byte[] Receive(int size = 1024)
    {
        var ptr = Marshal.AllocCoTaskMem(size);
        var status = WinSockNative.recv(_socket, ptr, size, 0);
        if (status == -1)
        {
            Marshal.FreeCoTaskMem(ptr);
            throw new WinSockException("recv failed", WinSockNativeWsa.WSAGetLastError());
        }
        
        var buffer = new byte[status];
        Marshal.Copy(ptr, buffer, 0, status);
        Marshal.FreeCoTaskMem(ptr);
        
        return buffer;
    }

    public bool IsConnected()
    {
        try
        {
            IsDataAvailable();
        }
        catch (WsaException)
        {
            return false;
        }

        return true;
    }

    public bool IsDataAvailable()
    {
        /*var ptr = Marshal.AllocCoTaskMem(1);
        var status = WinSockNative.recv(_socket, ptr, 1, 2);
        if (status == -1)
        {
            Marshal.FreeCoTaskMem(ptr);
            throw new WinSockException("recv failed", WinSockNativeWsa.WSAGetLastError());
        }
        
        Marshal.FreeCoTaskMem(ptr);

        return status > 0;*/

        var reads = new FdSet
        {
            fd_count = 1,
            fd_array = new long[64]
        };
        reads.fd_array[0] = _socket;

        var timeout = new TimeVal
        {
            tv_usec = 0,
            tv_sec = 1
        };

        var status = WinSockNative.select(0, ref reads, IntPtr.Zero, IntPtr.Zero, ref timeout);

        return status > 0;
    }

    public void Dispose()
    {
        WinSockNative.CloseSocket(_socket);
        WinSockNativeWsa.WSACleanup();
    }
}