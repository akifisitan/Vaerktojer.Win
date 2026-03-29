using Vaerktojer.Win.GlobalHotKeys;
using Vaerktojer.Win.GlobalHotKeys.Native;
using Vaerktojer.Win.GlobalHotKeys.Native.Types;

namespace Vaerktojer.Win.GlobalHotKeys.Tests;

public sealed class HotKeyManagerTests
{
    [Fact]
    public void CreateAndDisposeManager()
    {
        using var manager = new HotKeyManager();
    }

    [Fact]
    public void SubscribeReturnsDisposableHandle()
    {
        using var manager = new HotKeyManager();

        using var subscription = manager.Subscribe(_ => { });

        Assert.NotNull(subscription);
    }

    [Fact]
    public void SubscribeAfterDisposeThrowsObjectDisposedException()
    {
        var manager = new HotKeyManager();
        manager.Dispose();

        Assert.Throws<ObjectDisposedException>(() => manager.Subscribe(_ => { }));
    }

    [Fact]
    public void DisposeSubscriptionStopsFutureCallbacks()
    {
        using var manager = new HotKeyManager();
        var callCount = 0;
        var handler = manager.Subscribe(_ => callCount++);

        handler.Dispose();

        Functions.PostMessage(
            manager.WindowHandle,
            (uint)WindowMessage.WM_HOTKEY,
            new IntPtr(123),
            IntPtr.Zero
        );

        Thread.Sleep(50);

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void RegisterTwoKeysAssignsSequentialIds()
    {
        using var manager = new HotKeyManager();
        using var registration1 = manager.Register(VirtualKeyCode.VK_F23, Modifiers.Shift);
        using var registration2 = manager.Register(VirtualKeyCode.VK_F24, Modifiers.Shift);

        Assert.True(registration1.IsSuccessful);
        Assert.True(registration2.IsSuccessful);
        Assert.Equal(0, registration1.Id);
        Assert.Equal(1, registration2.Id);
    }

    [Fact]
    public void RegisterReusesIdsAfterDispose()
    {
        using var manager = new HotKeyManager();
        var registration1 = manager.Register(
            VirtualKeyCode.VK_F23,
            Modifiers.Control | Modifiers.Shift
        );

        Assert.True(registration1.IsSuccessful);
        Assert.Equal(0, registration1.Id);

        registration1.Dispose();

        using var registration2 = manager.Register(
            VirtualKeyCode.VK_F24,
            Modifiers.Control | Modifiers.Shift
        );

        Assert.True(registration2.IsSuccessful);
        Assert.Equal(0, registration2.Id);
    }
}
