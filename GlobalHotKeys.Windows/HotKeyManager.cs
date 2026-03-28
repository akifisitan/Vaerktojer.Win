using GlobalHotKeys.Native.Types;
using static GlobalHotKeys.Native.Functions;
using WndClassExHelpers = GlobalHotKeys.Native.WNDCLASSEX;

namespace GlobalHotKeys;

public sealed class HotKeyManager : IDisposable
{
    private const uint HotKeyMsg = 0x0312u;
    private const uint RegisterHotKeyMsg = 0x0400u;
    private const uint UnregisterHotKeyMsg = 0x0401u;

    private readonly Lock _subscriptionLock = new();
    private readonly List<Action<HotKey>> _subscriptions = [];
    private readonly Thread _thread;
    private bool _disposed;

    public HotKeyManager()
    {
        (_thread, WindowHandle) = StartMessageLoop();
    }

    internal IntPtr WindowHandle { get; }

    public IRegistration Register(VirtualKeyCode key, Modifiers modifiers)
    {
        var result = SendMessage(
            WindowHandle,
            RegisterHotKeyMsg,
            new IntPtr((int)key),
            new IntPtr((int)modifiers)
        );

        return new Registration(WindowHandle, result);
    }

    public IDisposable Subscribe(Action<HotKey> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_subscriptionLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _subscriptions.Add(handler);
        }

        return new Subscription(this, handler);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        PostMessage(WindowHandle, (uint)WindowMessage.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
        _thread.Join();

        lock (_subscriptionLock)
        {
            _subscriptions.Clear();
        }
    }

    private (Thread Thread, IntPtr WindowHandle) StartMessageLoop()
    {
        var tcsWindowHandle = new TaskCompletionSource<IntPtr>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        var thread = new Thread(() => ThreadEntry(tcsWindowHandle))
        {
            Name = "GlobalHotKeyManager Message Loop",
        };
        thread.Start();

        return (thread, tcsWindowHandle.Task.GetAwaiter().GetResult());
    }

    private void ThreadEntry(TaskCompletionSource<IntPtr> tcsWindowHandle)
    {
        var hInstance = GetModuleHandle(null);
        var registrations = new Dictionary<int, HotKey>();
        var wndClassEx = default(GlobalHotKeys.Native.Types.WNDCLASSEX);
        var registeredClass = (ushort)0;
        var hWnd = IntPtr.Zero;
        WndProc? wndProc = null;

        try
        {
            wndProc = new WndProc(MessageHandler);
            wndClassEx = WndClassExHelpers.FromWndProc(wndProc);
            registeredClass = RegisterClassEx(ref wndClassEx);

            hWnd = CreateWindowEx(
                0,
                registeredClass,
                null,
                WindowStyle.WS_OVERLAPPED,
                0,
                0,
                640,
                480,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            tcsWindowHandle.SetResult(hWnd);
            MessageLoop(hWnd);
        }
        catch (Exception ex)
        {
            tcsWindowHandle.TrySetException(ex);
            throw;
        }
        finally
        {
            if (hWnd != IntPtr.Zero)
            {
                foreach (var id in registrations.Keys.ToArray())
                {
                    Unregister(hWnd, id);
                }

                DestroyWindow(hWnd);
            }

            if (!string.IsNullOrEmpty(wndClassEx.lpszClassName))
            {
                UnregisterClass(wndClassEx.lpszClassName, hInstance);
            }

            GC.KeepAlive(wndProc);
        }

        return;

        int? NextId()
        {
            for (var id = 0x0000; id <= 0xBFFF; id++)
            {
                if (!registrations.ContainsKey(id))
                {
                    return id;
                }
            }

            return null;
        }

        bool RegisterHotKeyCore(
            IntPtr windowHandle,
            VirtualKeyCode hotKey,
            Modifiers hotKeyModifiers,
            int id
        )
        {
            if (!RegisterHotKey(windowHandle, id, hotKeyModifiers, hotKey))
            {
                return false;
            }

            registrations.Add(id, new HotKey(id, hotKeyModifiers, hotKey));
            return true;
        }

        bool Unregister(IntPtr windowHandle, int id)
        {
            if (!registrations.TryGetValue(id, out var registration))
            {
                return false;
            }

            if (!UnregisterHotKey(windowHandle, registration.Id))
            {
                return false;
            }

            registrations.Remove(id);
            return true;
        }

        IntPtr MessageHandler(IntPtr windowHandle, uint message, IntPtr wParam, IntPtr lParam)
        {
            switch (message)
            {
                case RegisterHotKeyMsg:
                {
                    var hotKey = (VirtualKeyCode)wParam.ToInt32();
                    var hotKeyModifiers = (Modifiers)lParam.ToInt32();
                    var id = NextId();

                    if (id is int registrationId)
                    {
                        return RegisterHotKeyCore(
                            windowHandle,
                            hotKey,
                            hotKeyModifiers,
                            registrationId
                        )
                            ? new IntPtr(registrationId)
                            : new IntPtr(-1);
                    }

                    return IntPtr.Zero;
                }

                case UnregisterHotKeyMsg:
                {
                    var id = wParam.ToInt32();
                    return Unregister(windowHandle, id) ? new IntPtr(id) : new IntPtr(-1);
                }

                case HotKeyMsg:
                {
                    if (registrations.TryGetValue(wParam.ToInt32(), out var hotKey))
                    {
                        PublishHotKey(hotKey);
                    }

                    return new IntPtr(1);
                }

                default:
                    return DefWindowProc(windowHandle, message, wParam, lParam);
            }
        }
    }

    private void PublishHotKey(HotKey hotKey)
    {
        Action<HotKey>[] handlers;

        lock (_subscriptionLock)
        {
            handlers = _subscriptions.ToArray();
        }

        foreach (var handler in handlers)
        {
            handler(hotKey);
        }
    }

    private void Unsubscribe(Action<HotKey> handler)
    {
        lock (_subscriptionLock)
        {
            _subscriptions.Remove(handler);
        }
    }

    private static void MessageLoop(IntPtr windowHandle)
    {
        var message = default(tagMSG);
        var result = 0;

        while ((result = GetMessage(ref message, windowHandle, 0u, 0u)) != -1 && result != 0)
        {
            TranslateMessage(ref message);
            DispatchMessage(ref message);
        }
    }

    private sealed class Registration(IntPtr windowHandle, IntPtr result) : IRegistration
    {
        private bool _disposed;

        public bool IsSuccessful => result.ToInt32() != -1;

        public int Id => result.ToInt32();

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            var id = result.ToInt32();
            if (id == -1)
            {
                return;
            }

            SendMessage(windowHandle, UnregisterHotKeyMsg, new IntPtr(id), IntPtr.Zero);
        }
    }

    private sealed class Subscription(HotKeyManager manager, Action<HotKey> handler) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            manager.Unsubscribe(handler);
        }
    }
}
