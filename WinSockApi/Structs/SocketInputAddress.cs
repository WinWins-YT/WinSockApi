using System.Runtime.InteropServices;

namespace WinSockApi.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct SocketInputAddress
{
    internal ushort sin_family;
    internal ushort sin_port;
    internal InputAddress sin_addr;
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    internal char[] sin_zero;
}

[StructLayout(LayoutKind.Sequential)]
internal struct InputAddress
{
    internal InputAddressUnion S_un;
}

[StructLayout(LayoutKind.Sequential)]
internal struct InputAddressB
{
    internal byte s_b1;
    internal byte s_b2;
    internal byte s_b3;
    internal byte s_b4;
}

[StructLayout(LayoutKind.Sequential)]
internal struct InputAddressW
{
    internal ushort s_w1;
    internal ushort s_w2;
}

[StructLayout(LayoutKind.Sequential)]
internal struct InputAddressUnion
{
    internal InputAddressB S_un_b;
    internal InputAddressW S_un_w;
    internal ulong S_addr;
}