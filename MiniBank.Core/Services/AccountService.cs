﻿using MiniBank.Core.Entities;
using MiniBank.Core.Services.Interfaces;

namespace MiniBank.Core.Services;

public class AccountService : IAccountService
{
    public Account GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Account> GetAll()
    {
        throw new NotImplementedException();
    }

    public Guid CreateAccount(Account account)
    {
        throw new NotImplementedException();
    }

    public void CloseAccount(Guid id)
    {
        throw new NotImplementedException();
    }

    public double CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId)
    {
        throw new NotImplementedException();
    }

    public Guid MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId)
    {
        throw new NotImplementedException();
    }
}