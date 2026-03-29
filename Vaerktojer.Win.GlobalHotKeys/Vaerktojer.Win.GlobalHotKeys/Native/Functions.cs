using System.Runtime.InteropServices;
using Vaerktojer.Win.GlobalHotKeys.Native.Types;
using NativeWndClassEx = Vaerktojer.Win.GlobalHotKeys.Native.Types.WNDCLASSEX;

namespace Vaerktojer.Win.GlobalHotKeys.Native;

internal static partial class Functions
{
    private const string Kernel32 = "Kernel32";
    private const string User32 = "User32";

    [LibraryImport(
        Kernel32,
        EntryPoint = "GetModuleHandleW",
        SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16
    )]
    public static partial IntPtr GetModuleHandle(string? lpModuleName);

    public static unsafe ushort RegisterClassEx(ref NativeWndClassEx lpwcx)
    {
        fixed (char* menuName = lpwcx.lpszMenuName)
        fixed (char* className = lpwcx.lpszClassName)
        {
            var nativeWndClassEx = new SourceGeneratorWndClassEx
            {
                cbSize = lpwcx.cbSize,
                style = lpwcx.style,
                lpfnWndProc = lpwcx.lpfnWndProc is null
                    ? IntPtr.Zero
                    : Marshal.GetFunctionPointerForDelegate(lpwcx.lpfnWndProc),
                cbClsExtra = lpwcx.cbClsExtra,
                cbWndExtra = lpwcx.cbWndExtra,
                hInstance = lpwcx.hInstance,
                hIcon = lpwcx.hIcon,
                hCursor = lpwcx.hCursor,
                hbrBackground = lpwcx.hbrBackground,
                lpszMenuName = menuName,
                lpszClassName = className,
                hIconSm = lpwcx.hIconSm,
            };

            return RegisterClassExCore(in nativeWndClassEx);
        }
    }

    [LibraryImport(User32, EntryPoint = "RegisterClassExW", SetLastError = true)]
    private static unsafe partial ushort RegisterClassExCore(in SourceGeneratorWndClassEx lpwcx);

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

    [LibraryImport(User32, EntryPoint = "DefWindowProcW")]
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

    [LibraryImport(User32, EntryPoint = "GetMessageW", SetLastError = true)]
    public static partial int GetMessage(
        ref tagMSG lpMsg,
        IntPtr hwnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax
    );

    [LibraryImport(User32, EntryPoint = "PostMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TranslateMessage(ref tagMSG lpMsg);

    [LibraryImport(User32, EntryPoint = "DispatchMessageW", SetLastError = true)]
    public static partial IntPtr DispatchMessage(ref tagMSG lpMsg);

    [LibraryImport(User32, EntryPoint = "SendMessageW", SetLastError = true)]
    public static partial IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct SourceGeneratorWndClassEx
    {
        public int cbSize;
        public int style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public char* lpszMenuName;
        public char* lpszClassName;
        public IntPtr hIconSm;
    }
}
