namespace CPG.Domain.SharedKernel;

public class Enums
{
    public enum ApplicationSettingEntityType
    {
        IDPCredential = 1,
    }

    public enum UserRoleType
    {
        Customer = 3
    }

    public enum OperationResult : byte
    {
        NotFound,
        Succeeded,
        Failed,
        Duplicate,
        NotValid
    }

        public enum CompanyPaymentMethodType
        {
            InternetPaymentGateway = 1,
            DirectDebit = 2,
            PaymentReceipt = 3
        }

        public enum UploadFileType
        {
            Company=1
        }
    }

public struct ResultData<T>(Enums.OperationResult operationResult)
{
    public T? Data { get; set; } = default(T);

    public Enums.OperationResult OperationResult { get; set; } = operationResult;

    public string? Error { get; set; } = null;
}
