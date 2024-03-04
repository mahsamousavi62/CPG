
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Serialization;
using CPG.Domain.SharedKernel;
using System;

namespace CPG.Application.UseCases.NeoBankServices.ViewModels;
public class UserDepositBalanceViewModel
{
    public decimal? BalanceAmount { get; set; }
    public string CustomerSureName { get; set; }
    public string CardNumber { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public Enums.NeoBankDepositStatus Status { get; set; }
}



