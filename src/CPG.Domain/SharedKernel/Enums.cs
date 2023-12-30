using CPG.Domain.AggregateModels.TransactionAggregate;

namespace CPG.Domain.SharedKernel;

public class Enums
{
    public enum ApplicationSettingEntityType
    {
        IDPCredential = 1,
        Minio = 3,
    }

    public enum IpgRedirectionMethodType : byte
    {
        CPGReferencePage = 1,
        RayanReferencePage = 2

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

    public enum TransactionType : byte
    {
        IPG = 1,
        DirectDebit = 2
    }

    public enum IPGTransactionStatus : byte
    {
        WaitingResponseFromIPG = 0,
    }

    public enum TransactionStatus : byte
    {
        InPrgress = 0,
    }

    public enum PaymentStatus
    {
        CanceledByUser=2,
        InProgress = 3,
        TransactionWaitingForVerification=4,
        TransactionSucceed = 8,
        TransactionFailed = 9,
    }
}

public struct ResultData<T>(Enums.OperationResult operationResult)
{
    public T? Data { get; set; } = default(T);

    public Enums.OperationResult OperationResult { get; set; } = operationResult;

    public string? Error { get; set; } = null;
}
