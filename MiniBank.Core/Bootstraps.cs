using Microsoft.Extensions.DependencyInjection;
using MiniBank.Core.Interfaces;
using MiniBank.Core.Services;

namespace MiniBank.Core;

public static class Bootstraps
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyRateConversionService, CurrencyRateConversionService>();

        return services;
    }
}