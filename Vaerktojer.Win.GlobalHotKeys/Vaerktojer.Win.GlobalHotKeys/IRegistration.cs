namespace Vaerktojer.Win.GlobalHotKeys;

internal interface IRegistration : IDisposable
{
    bool IsSuccessful { get; }

    int Id { get; }
}
