using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using MiniBank.Core.Domain.Accounts.Services;
using MiniBank.Core.Domain.Currencies.Services;
using MiniBank.Core.Domain.Users.Services;

namespace MiniBank.Core;

public static class Bootstraps
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyRateConversionService, CurrencyRateConversionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddFluentValidation().AddValidatorsFromAssembly(typeof(UserService).Assembly);

        return services;
    }
}