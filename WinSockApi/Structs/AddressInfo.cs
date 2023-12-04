using System.Runtime.InteropServices;

namespace WinSockApi.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct AddressInfo
{
    internal int ai_flags;
    internal int ai_family;
    internal int ai_socktype;
    internal int ai_protocol;
    internal ulong ai_addrlen;

    [MarshalAs(UnmanagedType.LPStr)]
    internal string ai_canonname;
    
    internal IntPtr ai_addr;
    internal IntPtr ai_next;
}