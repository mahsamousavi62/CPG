namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;

public class ShowResponse : ResponseBase
{
    public int GrantStatus { get; set; }
    public string GrantMessage { get; set; }
    public GrantData GrantData { get; set; }
}

public class GrantData
{
    public string Id { get; set; }
    public string Token { get; set; }
    public string BankCode { get; set; }
    public string CallbackUrl { get; set; }
    public int Count { get; set; }
    public decimal Limit { get; set; }
    public string Mobile { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string NationalCode { get; set; }
    public string ExpirationDate { get; set; }
    public string Status { get; set; }
    public string AccountNumber { get; set; }
    public string Pan { get; set; }
    public string CreatedAt { get; set; }
    public string RevokedAt { get; set; }
}