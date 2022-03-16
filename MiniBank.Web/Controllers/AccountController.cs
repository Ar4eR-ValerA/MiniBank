using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Entities;
using MiniBank.Core.Services;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Web.Dtos;

namespace MiniBank.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController()
    {
        _accountService = new AccountService();
    }

    [HttpGet("GetAccountById")]
    public AccountDto GetAccountById([FromQuery] Guid id)
    {
        var account = _accountService.GetById(id);

        return new AccountDto(
            account.Id,
            account.UserId,
            account.Balance,
            account.Currency,
            account.IsActive,
            account.DateOpened,
            account.DateClosed);
    }
    
    [HttpGet("GetAllAccounts")]
    public IEnumerable<AccountDto> GetAllAccounts()
    {
        var accounts = _accountService.GetAll();

        return accounts.Select(a => new AccountDto(
            a.Id,
            a.UserId,
            a.Balance,
            a.Currency,
            a.IsActive,
            a.DateOpened,
            a.DateClosed));
    }
    
    [HttpPost("CreateAccount")]
    public Guid CreateAccount(
        [FromQuery] 
        Guid userId, 
        double balance, 
        string currency, 
        bool isActive, 
        DateTime dateOpened, 
        DateTime dateClosed)
    {
        var account = new Account(Guid.Empty, userId, balance, currency, isActive, dateOpened, dateClosed);

        return _accountService.CreateAccount(account);
    }

    [HttpPut("CloseAccount")]
    public void CloseAccount(Guid id)
    {
        _accountService.CloseAccount(id);
    }

    [HttpGet("CalculateCommission")]
    public double CalculateCommission([FromQuery] double amount, Guid fromAccountId, Guid toAccountId)
    {
        return _accountService.CalculateCommission(amount, fromAccountId, toAccountId);
    }

    [HttpPost("MakeTransaction")]
    public Guid MakeTransaction([FromQuery] double amount, Guid fromAccountId, Guid toAccountId)
    {
        return _accountService.MakeTransaction(amount, fromAccountId, toAccountId);
    }
}