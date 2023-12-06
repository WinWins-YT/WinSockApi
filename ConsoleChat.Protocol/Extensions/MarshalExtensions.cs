using System.Runtime.InteropServices;

namespace ConsoleChat.Protocol.Extensions;

public static class MarshalExtensions
{
    public static byte[] ToByteArray<T>(this T str) where T : struct
    {
        var size = Marshal.SizeOf(str);
        var arr = new byte[size];

        var ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return arr;
    }
    
    public static T FromByteArray<T>(this byte[] arr) where T : struct
    {
        T str;
        var size = arr.Length;
        var ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = Marshal.PtrToStructure<T>(ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return str;
    }
}