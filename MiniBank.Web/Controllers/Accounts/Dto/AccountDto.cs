﻿namespace MiniBank.Web.Controllers.Accounts.Dto;

public class AccountDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double Balance { get; set; }
    public string Currency { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateOpened { get; set; }
    public DateTime? DateClosed { get; set; }
}