namespace CPG.Domain.SharedKernel;

public class Enums
{
    public enum ApplicationSettingEntityType
    {
        IDPCredential = 1,
        Minio = 3,
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

    public enum CompanyPaymentMethodType : short
    {
        InternetPaymentGateway = 1,
        DirectDebit = 2,
        PaymentReceipt = 3
    }

    public enum UploadFromEntityType
    {
        Company = 1,
        Bank = 2
    }

    public enum ProviderType : byte
    {
        Vandar = 1,
    }
}

public struct ResultData<T>(Enums.OperationResult operationResult)
{
    public T? Data { get; set; } = default(T);

    public Enums.OperationResult OperationResult { get; set; } = operationResult;

    public string? Error { get; set; } = null;
}
