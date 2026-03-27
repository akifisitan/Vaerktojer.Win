using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Vaerktojer.Win.Clipboard;

public static class DIRegistrations
{
    public static IServiceCollection AddVaerktojerWinClipboard(this IServiceCollection services)
    {
        services.AddSingleton<IWindowsClipboard, WindowsClipboard>();

        services.TryAddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.TryAddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

        return services;
    }
}
