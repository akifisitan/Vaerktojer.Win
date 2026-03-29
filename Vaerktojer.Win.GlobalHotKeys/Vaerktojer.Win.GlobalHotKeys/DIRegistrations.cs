using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Vaerktojer.Win.GlobalHotKeys;

public static class DIRegistrations
{
    public static IServiceCollection AddVaerktojerWinGlobalHotKeys(this IServiceCollection services)
    {
        services.TryAddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.TryAddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

        services.AddSingleton<IGlobalHotKeyManager, GlobalHotKeyManager>();

        return services;
    }
}
