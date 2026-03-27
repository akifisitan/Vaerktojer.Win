using System.Runtime.InteropServices;
using GlobalHotKeys.Native.Types;

namespace GlobalHotKeys.Native;

using NativeWndClassEx = GlobalHotKeys.Native.Types.WNDCLASSEX;

public static class WNDCLASSEX
{
    public static NativeWndClassEx init(IntPtr hInstance, string className, WndProc wndProc)
    {
        return new NativeWndClassEx
        {
            cbSize = Marshal.SizeOf<NativeWndClassEx>(),
            hInstance = hInstance,
            lpfnWndProc = wndProc,
            lpszClassName = className,
        };
    }

    public static NativeWndClassEx fromWndProc(WndProc wndProc)
    {
        var hInstance = Functions.GetModuleHandle(null);
        var className = $"GlobalHotKeys-{Guid.NewGuid():N}";
        return init(hInstance, className, wndProc);
    }
}
