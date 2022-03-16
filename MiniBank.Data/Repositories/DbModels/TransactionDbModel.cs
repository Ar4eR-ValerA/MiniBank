﻿namespace MiniBank.Data.Repositories.DbModels;

public class TransactionDbModel
{
    public TransactionDbModel(Guid id, double amount, string currency, Guid fromAccountId, Guid toAccountId)
    {
        Id = id;
        Amount = amount;
        Currency = currency ?? throw new Exception("Currency is null");
        FromAccountId = fromAccountId;
        ToAccountId = toAccountId;
    }

    public Guid Id { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
}