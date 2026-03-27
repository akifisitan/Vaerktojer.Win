using GlobalHotKeys.Native.Types;

namespace GlobalHotKeys;

public sealed record HotKey(int Id, Modifiers Modifiers, VirtualKeyCode Key);
