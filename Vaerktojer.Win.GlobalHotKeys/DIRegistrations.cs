using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Vaerktojer.Win.GlobalHotKeys;

public static class DIRegistrations
{
    public static IServiceCollection AddVaerktojerGlobalHotKeysWin(this IServiceCollection services)
    {
        services.AddSingleton<GlobalHotKeyManager>();

        services.TryAddSingleton(typeof(NullLogger<>));
        services.TryAddSingleton(typeof(NullLoggerFactory));

        return services;
    }
}
