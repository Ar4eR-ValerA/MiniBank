using Microsoft.Extensions.DependencyInjection;
using MiniBank.Core.Services;
using MiniBank.Core.Services.Interfaces;

namespace MiniBank.Core;

public static class Bootstraps
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyRateConversionService, CurrencyRateConversionService>();

        return services;
    }
}