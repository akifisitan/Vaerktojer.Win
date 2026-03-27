using System.Runtime.InteropServices;
using GlobalHotKeys.Native;
using GlobalHotKeys.Native.Types;
using WndClassExHelpers = GlobalHotKeys.Native.WNDCLASSEX;

namespace GlobalHotKeys.Windows.Tests;

public sealed class WndClassExTests
{
    [Fact]
    public void InitSetsExpectedFields()
    {
        static IntPtr MessageHandler(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam) =>
            IntPtr.Zero;

        var hInstance = new IntPtr(3);
        const string className = "class";
        var wndProc = new WndProc(MessageHandler);

        var result = WndClassExHelpers.Init(hInstance, className, wndProc);

        Assert.Equal(hInstance, result.hInstance);
        Assert.NotEqual(0, result.cbSize);
        Assert.NotNull(result.lpfnWndProc);
        Assert.Equal(className, result.lpszClassName);
        Assert.Equal(Marshal.SizeOf<GlobalHotKeys.Native.Types.WNDCLASSEX>(), result.cbSize);
    }
}
