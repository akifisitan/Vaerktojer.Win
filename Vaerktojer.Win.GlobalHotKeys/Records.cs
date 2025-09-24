using GlobalHotKeys.Native.Types;

namespace Vaerktojer.Win.GlobalHotKeys;

public record HotKeyRegistration(
    Modifiers Modifiers,
    VirtualKeyCode VirtualKeyCode,
    Func<Task> Callback
);
