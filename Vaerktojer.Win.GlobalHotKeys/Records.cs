using GlobalHotKeys.Native.Types;

namespace Vaerktojer.Win.GlobalHotKeys;

public sealed record HotKeyRegistration(
    Modifiers Modifiers,
    VirtualKeyCode VirtualKeyCode,
    Action Callback
);
