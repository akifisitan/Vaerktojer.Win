using GlobalHotKeys.Native;

namespace GlobalHotKeys.Windows.Tests;

public sealed class ModuleHandleTests
{
    [Fact]
    public void GetModuleHandleReturnsCurrentProcessHandle()
    {
        var handle = Functions.GetModuleHandle(null);

        Assert.NotEqual(IntPtr.Zero, handle);
    }
}
