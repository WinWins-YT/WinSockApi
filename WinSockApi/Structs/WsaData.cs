using System.Runtime.InteropServices;

namespace WinSockApi.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct WsaData
{
    public ushort wVersion;
    public ushort wHighVersion;

    public ushort iMaxSockets;
    public ushort iMaxUdpDg;
    public string lpVendorInfo;
    
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
    public string szDescription;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
    public string szSystemStatus;
}