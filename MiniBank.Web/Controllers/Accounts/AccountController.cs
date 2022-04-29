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
    public async Task<AccountDto> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _accountService.GetById(id, cancellationToken);

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
    public async Task<IReadOnlyList<AccountDto>> GetAll(CancellationToken cancellationToken = default)
    {
        var accounts = await _accountService.GetAll(cancellationToken);

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
    public Task<Guid> Create(AccountCreateDto accountCreateDto, CancellationToken cancellationToken = default)
    {
        var account = new Account
        {
            UserId = accountCreateDto.UserId,
            Balance = accountCreateDto.Balance,
            Currency = accountCreateDto.Currency
        };

        return _accountService.Create(account, cancellationToken);
    }

    [HttpPut("close/{id:guid}")]
    public Task Close(Guid id, CancellationToken cancellationToken = default)
    {
        return _accountService.Close(id, cancellationToken);
    }

    [HttpGet("calculate-commission")]
    public Task<double> CalculateCommission(
        double amount,
        Guid fromAccountId,
        Guid toAccountId,
        CancellationToken cancellationToken = default)
    {
        return _accountService.CalculateCommission(amount, fromAccountId, toAccountId, cancellationToken);
    }

    [HttpPost("make-transaction")]
    public Task<Guid> MakeTransaction(
        double amount,
        Guid fromAccountId,
        Guid toAccountId,
        CancellationToken cancellationToken = default)
    {
        return _accountService.MakeTransaction(amount, fromAccountId, toAccountId, cancellationToken);
    }
}