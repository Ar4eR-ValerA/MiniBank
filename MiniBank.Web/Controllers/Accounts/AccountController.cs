using Microsoft.AspNetCore.Mvc;
using MiniBank.Core.Domain.Accounts;
using MiniBank.Core.Domain.Accounts.Services;
using MiniBank.Web.Controllers.Accounts.Dto;

namespace MiniBank.Web.Controllers.Accounts;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("{id:guid}")]
    public async Task<AccountDto> GetById(Guid id)
    {
        var account = await _accountService.GetById(id);

        return new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance,
            Currency = account.Currency,
            IsActive = account.IsActive,
            DateOpened = account.DateOpened,
            DateClosed = account.DateClosed
        };
    }

    [HttpGet]
    public async Task<IReadOnlyList<AccountDto>> GetAll()
    {
        var accounts = await _accountService.GetAll();

        return accounts.Select(a => new AccountDto
        {
            Id = a.Id,
            UserId = a.UserId,
            Balance = a.Balance,
            Currency = a.Currency,
            IsActive = a.IsActive,
            DateOpened = a.DateOpened,
            DateClosed = a.DateClosed
        }).ToList();
    }

    [HttpPost("create")]
    public async Task<Guid> Create(AccountCreateDto accountCreateDto)
    {
        var account = new Account
        {
            UserId = accountCreateDto.UserId,
            Balance = accountCreateDto.Balance,
            Currency = accountCreateDto.Currency
        };

        return await _accountService.Create(account);
    }

    [HttpPut("close/{id:guid}")]
    public async Task Close(Guid id)
    {
        await _accountService.Close(id);
    }

    [HttpGet("calculate-commission")]
    public async Task<double> CalculateCommission(double amount, Guid fromAccountId, Guid toAccountId)
    {
        return await _accountService.CalculateCommission(amount, fromAccountId, toAccountId);
    }

    [HttpPost("make-transaction")]
    public async Task<Guid> MakeTransaction(double amount, Guid fromAccountId, Guid toAccountId)
    {
        return await _accountService.MakeTransaction(amount, fromAccountId, toAccountId);
    }
}