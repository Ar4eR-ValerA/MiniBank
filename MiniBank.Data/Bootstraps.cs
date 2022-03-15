using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniBank.Core.Interfaces;
using MiniBank.Data.Services;

namespace MiniBank.Data;

public static class Bootstraps
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ICurrencyRateProvider, CurrencyRateProvider>(options =>
        {
            options.BaseAddress = new Uri(configuration["Currencies"]);
        });

        return services;
    }
}