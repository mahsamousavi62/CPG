namespace CPG.Application.UseCases.DirectDebit.ViewModels;

public class ValidateGrantRequestViewModel
{
    public string Request { get; set; }
}

public class VandarValidateGrantViewModel
{
    public string Token { get; set; }
    public string Status { get; set; }
    public string ErrorCode { get; set; }
    public string AuthorizationId { get; set; }
}