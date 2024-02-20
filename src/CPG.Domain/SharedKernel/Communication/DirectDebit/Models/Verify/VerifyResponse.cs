namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Verify;

public class VerifyResponse : ResponseBase
{
    public int GrantStatus { get; set; }
    public string GrantMessage { get; set; }
}
