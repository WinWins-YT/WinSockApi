using System.Runtime.InteropServices;

namespace WinSockApi.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct FdSet
{
    internal uint fd_count;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    internal long[] fd_array;
}