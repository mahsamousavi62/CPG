using System.Collections.Generic;

namespace CPG.Application.UseCases.CharismaCard.ViewModels;

public class CharismaCardBalanceViewModel
{
    public bool IsSuccess { get; set; }
    public List<CharismaCardAccountViewModel> Accounts { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorDescription { get; set; }
}

public class CharismaCardAccountViewModel
{
    public decimal Balance { get; set; }
    public string CustomerFirstName { get; set; }
    public string CustomerLastName { get; set; }
    public string Iban { get; set; }
    public string CardNumber { get; set; }
    public string DepositNumber { get; set; }
    public string UrlAliasName { get; set; }
}