namespace CPG.Domain.SharedKernel;

public class Enums
{
    public enum ApplicationSettingEntityType
    {
        /// <summary>IDPCredential/// </summary>
        IDPCredential = 1,

        /// <summary>
        /// Minio
        /// </summary>
        Minio = 3,
    }

    public enum UserRoleType
    {
        SuperAdmin = 1,
        CompanyUser = 2,
        CustomerUser = 3
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
        Bank = 2,
        Provider = 3,
        Application = 4,
    }

    public enum ProviderType : byte
    {
        Vandar = 1,
        AsanPardakht = 2,
    }

    public enum ServiceType : byte
    {
        AsanPardakhtToken = 1,
        AsanPardakhtTransResult = 2,
    }
}

public struct ResultData<T>(Enums.OperationResult operationResult)
{
    public T? Data { get; set; } = default(T);

    public Enums.OperationResult OperationResult { get; set; } = operationResult;

    public string? Error { get; set; } = null;
}
