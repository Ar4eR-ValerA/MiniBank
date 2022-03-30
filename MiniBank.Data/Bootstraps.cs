﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniBank.Core.Domain.Accounts.Repositories;
using MiniBank.Core.Domain.Currencies.Providers;
using MiniBank.Core.Domain.Transactions.Repositories;
using MiniBank.Core.Domain.Users.Repositories;
using MiniBank.Data.Accounts.Repositories;
using MiniBank.Data.Contexts;
using MiniBank.Data.CurrencyRates.Providers;
using MiniBank.Data.Transactions.Repositories;
using MiniBank.Data.Users.Repositories;

namespace MiniBank.Data;

public static class Bootstraps
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ICurrencyRateProvider, CurrencyRateProvider>(options =>
        {
            options.BaseAddress = new Uri(configuration["Currencies"]);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddDbContext<MiniBankContext>(options =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Database=MiniBank1.0;Username=postgres;Password=123456");
        });
        
        return services;
    }
}