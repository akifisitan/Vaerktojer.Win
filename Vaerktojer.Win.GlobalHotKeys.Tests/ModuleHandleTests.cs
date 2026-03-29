using Vaerktojer.Win.GlobalHotKeys.Native;

namespace Vaerktojer.Win.GlobalHotKeys.Tests;

public sealed class ModuleHandleTests
{
    [Fact]
    public void GetModuleHandleReturnsCurrentProcessHandle()
    {
        var handle = Functions.GetModuleHandle(null);

        Assert.NotEqual(IntPtr.Zero, handle);
    }
}
