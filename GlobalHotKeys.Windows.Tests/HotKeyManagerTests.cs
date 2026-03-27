using GlobalHotKeys.Native.Types;

namespace GlobalHotKeys.Windows.Tests;

public sealed class HotKeyManagerTests
{
    [Fact]
    public void CreateAndDisposeManager()
    {
        using var manager = new HotKeyManager();
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
