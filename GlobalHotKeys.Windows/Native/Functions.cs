using System.Runtime.InteropServices;
using GlobalHotKeys.Native.Types;

namespace GlobalHotKeys.Native;

using NativeWndClassEx = GlobalHotKeys.Native.Types.WNDCLASSEX;

public static class Functions
{
    private const string Kernel32 = "Kernel32";
    private const string User32 = "User32";

    [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport(
        User32,
        SetLastError = true,
        CharSet = CharSet.Unicode,
        EntryPoint = "RegisterClassExW"
    )]
    public static extern ushort RegisterClassEx(ref NativeWndClassEx lpwcx);

    [DllImport(
        User32,
        SetLastError = true,
        CharSet = CharSet.Unicode,
        EntryPoint = "UnregisterClassW"
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

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

    [DllImport(
        User32,
        SetLastError = true,
        CharSet = CharSet.Unicode,
        EntryPoint = "CreateWindowExW"
    )]
    private static extern IntPtr CreateWindowExCore(
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

    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(IntPtr hwnd);

    [DllImport(User32)]
    public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(
        IntPtr hWnd,
        int id,
        Modifiers fsModifiers,
        VirtualKeyCode vk
    );

    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport(User32, SetLastError = true)]
    public static extern int GetMessage(
        ref tagMSG lpMsg,
        IntPtr hwnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax
    );

    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TranslateMessage(ref tagMSG lpMsg);

    [DllImport(User32, SetLastError = true)]
    public static extern IntPtr DispatchMessage(ref tagMSG lpMsg);

    [DllImport(User32, SetLastError = true)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
