using System.Runtime.InteropServices;
using GlobalHotKeys.Native.Types;
using NativeWndClassEx = GlobalHotKeys.Native.Types.WNDCLASSEX;

namespace GlobalHotKeys.Native;

internal static partial class Functions
{
    private const string Kernel32 = "Kernel32";
    private const string User32 = "User32";

    [LibraryImport(Kernel32, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr GetModuleHandle(string? lpModuleName);

    [LibraryImport(User32, EntryPoint = "RegisterClassExW", SetLastError = true)]
    public static partial ushort RegisterClassEx(ref NativeWndClassEx lpwcx);

    [LibraryImport(
        User32,
        EntryPoint = "UnregisterClassW",
        SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnregisterClass(string lpClassName, IntPtr hInstance);

    public static IntPtr CreateWindowEx(
        int dwExStyle,
        uint lpClassName,
        string? lpWindowName,
        WindowStyle dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam
    ) =>
        CreateWindowExCore(
            dwExStyle,
            new IntPtr(unchecked((int)lpClassName)),
            lpWindowName,
            dwStyle,
            x,
            y,
            nWidth,
            nHeight,
            hWndParent,
            hMenu,
            hInstance,
            lpParam
        );

    public static IntPtr CreateWindowEx(
        int dwExStyle,
        ushort lpClassName,
        string? lpWindowName,
        WindowStyle dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam
    ) =>
        CreateWindowExCore(
            dwExStyle,
            new IntPtr(lpClassName),
            lpWindowName,
            dwStyle,
            x,
            y,
            nWidth,
            nHeight,
            hWndParent,
            hMenu,
            hInstance,
            lpParam
        );

    [LibraryImport(
        User32,
        EntryPoint = "CreateWindowExW",
        SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16
    )]
    private static partial IntPtr CreateWindowExCore(
        int dwExStyle,
        IntPtr lpClassName,
        string? lpWindowName,
        WindowStyle dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam
    );

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DestroyWindow(IntPtr hwnd);

    [LibraryImport(User32)]
    public static partial IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool RegisterHotKey(
        IntPtr hWnd,
        int id,
        Modifiers fsModifiers,
        VirtualKeyCode vk
    );

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnregisterHotKey(IntPtr hWnd, int id);

    [LibraryImport(User32, SetLastError = true)]
    public static partial int GetMessage(
        ref tagMSG lpMsg,
        IntPtr hwnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax
    );

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TranslateMessage(ref tagMSG lpMsg);

    [LibraryImport(User32, SetLastError = true)]
    public static partial IntPtr DispatchMessage(ref tagMSG lpMsg);

    [LibraryImport(User32, SetLastError = true)]
    public static partial IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
