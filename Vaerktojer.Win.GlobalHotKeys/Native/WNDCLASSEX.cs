using System.Runtime.InteropServices;
using Vaerktojer.Win.GlobalHotKeys.Native.Types;
using NativeWndClassEx = Vaerktojer.Win.GlobalHotKeys.Native.Types.WNDCLASSEX;

namespace Vaerktojer.Win.GlobalHotKeys.Native;

internal static class WNDCLASSEX
{
    public static NativeWndClassEx Init(IntPtr hInstance, string className, WndProc wndProc)
    {
        return new NativeWndClassEx
        {
            cbSize = Marshal.SizeOf<NativeWndClassEx>(),
            hInstance = hInstance,
            lpfnWndProc = wndProc,
            lpszClassName = className,
        };
    }

    public static NativeWndClassEx FromWndProc(WndProc wndProc)
    {
        var hInstance = Functions.GetModuleHandle(null);
        var className = $"GlobalHotKeys-{Guid.NewGuid():N}";
        return Init(hInstance, className, wndProc);
    }
}
