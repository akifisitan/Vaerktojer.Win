using System.Collections.Concurrent;
using GlobalHotKeys;
using GlobalHotKeys.Native.Types;

var consoleLock = new object();
using var exitSignal = new ManualResetEventSlim(false);
using var manager = new HotKeyManager();

var demoHotKeys = new[]
{
    new DemoHotKey(
        "Action 1",
        Modifiers.Control | Modifiers.Shift | Modifiers.NoRepeat,
        VirtualKeyCode.VK_F10
    ),
    new DemoHotKey(
        "Action 2",
        Modifiers.Control | Modifiers.Shift | Modifiers.NoRepeat,
        VirtualKeyCode.VK_F11
    ),
};

var pressCounts = new ConcurrentDictionary<int, int>();
var registrations = new List<RegistrationEntry>();

foreach (var demoHotKey in demoHotKeys)
{
    var registration = manager.Register(demoHotKey.Key, demoHotKey.Modifiers);
    registrations.Add(new RegistrationEntry(demoHotKey, registration));
}

using var subscription = manager.HotKeyPressed.Subscribe(hotKey =>
{
    var registration = registrations.FirstOrDefault(entry => entry.Registration.Id == hotKey.Id);
    var label = registration?.Definition.Label ?? "Unknown";
    var combo = registration is null
        ? HotKeyFormatting.FormatHotKey(hotKey.Modifiers, hotKey.Key)
        : registration.Definition.DisplayText;
    var count = pressCounts.AddOrUpdate(hotKey.Id, 1, static (_, current) => current + 1);

    WriteLine(
        $"[{DateTime.Now:HH:mm:ss}] {label} triggered via {combo} (Id={hotKey.Id}, Count={count}, Thread={Environment.CurrentManagedThreadId})"
    );
});

Console.CancelKeyPress += OnCancelKeyPress;

try
{
    PrintHeader();
    PrintRegistrations(registrations);
    WriteLine("Press the registered hotkeys anywhere in Windows.");
    WriteLine("Return to this console and press Q or Ctrl+C to quit.");

    WaitForExitRequest();
}
finally
{
    Console.CancelKeyPress -= OnCancelKeyPress;

    foreach (var registration in registrations)
    {
        registration.Registration.Dispose();
    }

    WriteLine("Global hotkeys unregistered. Goodbye.");
}

return;

void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs args)
{
    args.Cancel = true;
    WriteLine("Ctrl+C received. Shutting down...");
    exitSignal.Set();
}

void PrintHeader()
{
    WriteLine("GlobalHotKeys demo");
    WriteLine(new string('-', 72));
}

void PrintRegistrations(IEnumerable<RegistrationEntry> entries)
{
    foreach (var entry in entries)
    {
        var status = entry.Registration.IsSuccessful ? "OK" : "FAILED";
        var idText = entry.Registration.IsSuccessful ? entry.Registration.Id.ToString() : "-";
        WriteLine(
            $"{entry.Definition.Label, -8} {entry.Definition.DisplayText, -28} Id={idText, -3} Status={status}"
        );
    }
}

void WaitForExitRequest()
{
    if (Console.IsInputRedirected)
    {
        WriteLine(
            "Console input is redirected; send q on stdin or close the input stream to quit."
        );

        while (!exitSignal.IsSet)
        {
            var line = Console.ReadLine();
            if (line is null)
            {
                WriteLine("Input stream ended. Shutting down...");
                exitSignal.Set();
                break;
            }

            if (string.Equals(line.Trim(), "q", StringComparison.OrdinalIgnoreCase))
            {
                WriteLine("Quit requested from redirected input.");
                exitSignal.Set();
            }
        }

        return;
    }

    while (!exitSignal.IsSet)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Q)
        {
            WriteLine("Quit requested from console input.");
            exitSignal.Set();
        }
    }
}

void WriteLine(string message)
{
    lock (consoleLock)
    {
        Console.WriteLine(message);
    }
}

sealed record DemoHotKey(string Label, Modifiers Modifiers, VirtualKeyCode Key)
{
    public string DisplayText => HotKeyFormatting.FormatHotKey(Modifiers, Key);
}

sealed record RegistrationEntry(DemoHotKey Definition, IRegistration Registration);

static class HotKeyFormatting
{
    public static string FormatHotKey(Modifiers modifiers, VirtualKeyCode key)
    {
        var parts = new List<string>();

        if (modifiers.HasFlag(Modifiers.Control))
        {
            parts.Add("Ctrl");
        }

        if (modifiers.HasFlag(Modifiers.Shift))
        {
            parts.Add("Shift");
        }

        if (modifiers.HasFlag(Modifiers.Alt))
        {
            parts.Add("Alt");
        }

        if (modifiers.HasFlag(Modifiers.Win))
        {
            parts.Add("Win");
        }

        if (modifiers.HasFlag(Modifiers.NoRepeat))
        {
            parts.Add("NoRepeat");
        }

        parts.Add(key.ToString().Replace("VK_", string.Empty));
        return string.Join('+', parts);
    }
}
