namespace Vaerktojer.Win.GlobalHotKeys;

public interface IGlobalHotKeyManager : IDisposable
{
    void RegisterHotKeys(IList<HotKeyRegistration> entries);
    void UnregisterHotKeys();
}
