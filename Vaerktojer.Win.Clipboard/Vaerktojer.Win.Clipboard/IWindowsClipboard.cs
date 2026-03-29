namespace Vaerktojer.Win.Clipboard;

public interface IWindowsClipboard
{
    string GetClipboardText();
    void SetClipboardText(string text);
}
