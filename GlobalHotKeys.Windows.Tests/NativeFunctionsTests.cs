using GlobalHotKeys.Native;
using GlobalHotKeys.Native.Types;

namespace GlobalHotKeys.Windows.Tests;

public sealed class NativeFunctionsTests
{
    [Fact]
    public void PostMessageAndGetMessageRoundTrip()
    {
        var messageId = (uint)WindowMessage.WM_HOTKEY;
        var wParam = new IntPtr(2);
        var lParam = new IntPtr(3);

        var postResult = Functions.PostMessage(IntPtr.Zero, messageId, wParam, lParam);
        var message = default(tagMSG);

        var result = Functions.GetMessage(ref message, IntPtr.Zero, 0u, 0u);

        Assert.True(postResult);
        Assert.True(result > 0);
        Assert.Equal(messageId, message.message);
        Assert.Equal(wParam, message.wParam);
        Assert.Equal(lParam, message.lParam);
    }
}
