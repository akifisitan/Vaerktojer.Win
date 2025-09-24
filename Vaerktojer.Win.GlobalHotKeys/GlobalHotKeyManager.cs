using GlobalHotKeys;

namespace Vaerktojer.Win.GlobalHotKeys;

public sealed class GlobalHotKeyManager : IDisposable
{
    private readonly HotKeyManager _hotKeyManager = new();
    private readonly List<IDisposable?> _hotKeyRegistrations = [];
    private IDisposable? _observableHandle;
    private bool _registered = false;

    public void RegisterHotKeys(List<HotKeyRegistration> entries)
    {
        if (_registered)
        {
            throw new Exception(
                $"Hotkeys can only be registered once per {nameof(GlobalHotKeyManager)} instance. Create a new instance to register new hotkeys."
            );
        }

        InitializeHotKeyManager(entries);
        _registered = true;
    }

    private void InitializeHotKeyManager(List<HotKeyRegistration> registrations)
    {
        foreach (var registration in registrations)
        {
            _hotKeyRegistrations.Add(
                _hotKeyManager.Register(registration.VirtualKeyCode, registration.Modifiers)
            );
        }

        _observableHandle = _hotKeyManager.HotKeyPressed.Subscribe(async hotKey =>
        {
            Console.WriteLine($"Hotkey pressed in thread: {Environment.CurrentManagedThreadId}");

            foreach (var entry in registrations)
            {
                if (entry.VirtualKeyCode == hotKey.Key && entry.Modifiers == hotKey.Modifiers)
                {
                    await entry.Callback().ConfigureAwait(false);
                }
            }
        });
    }

    public void Dispose()
    {
        _observableHandle?.Dispose();

        foreach (var registration in _hotKeyRegistrations)
        {
            registration?.Dispose();
        }

        _hotKeyManager.Dispose();
    }
}
