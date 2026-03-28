using GlobalHotKeys;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Vaerktojer.Win.GlobalHotKeys;

public sealed class GlobalHotKeyManager : IGlobalHotKeyManager, IDisposable
{
    private readonly ILogger<GlobalHotKeyManager> _logger;

    private readonly HotKeyManager _hotKeyManager = new();
    private readonly List<IDisposable?> _hotKeyRegistrations = [];
    private IDisposable? _observableHandle;
    private bool _registered = false;

    public GlobalHotKeyManager(ILogger<GlobalHotKeyManager> logger)
    {
        _logger = logger;
    }

    public void RegisterHotKeys(IList<HotKeyRegistration> entries)
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

    private void InitializeHotKeyManager(IList<HotKeyRegistration> registrations)
    {
        foreach (var registration in registrations)
        {
            _hotKeyRegistrations.Add(
                _hotKeyManager.Register(registration.VirtualKeyCode, registration.Modifiers)
            );
        }

        _observableHandle = _hotKeyManager.HotKeyPressed.Subscribe(hotKey =>
        {
            _logger.ZLogDebug(
                $"Hotkey pressed {hotKey} in thread: {Environment.CurrentManagedThreadId}"
            );

            foreach (var entry in registrations)
            {
                if (entry.VirtualKeyCode == hotKey.Key && entry.Modifiers == hotKey.Modifiers)
                {
                    _logger.ZLogDebug($"Invoking callback for {hotKey}");
                    entry.Callback();
                    break;
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
