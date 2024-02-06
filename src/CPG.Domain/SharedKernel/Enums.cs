using CPG.Domain.AggregateModels.TransactionAggregate;
using static CPG.Domain.SharedKernel.Enums;

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

    public enum PaymentMethodType : byte
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
        Sep = 3,
        Pec = 4,
    }

    public enum ServiceType : byte
    {
        AsanPardakhtToken = 1,
        AsanPardakhtTransResult = 2,
        AsanPardakhtVerify = 3,
        SepToken = 4,
        SepVerify = 5,
        PecToken = 6,
        PecVerify = 7,
        VandarToken = 8,
        VandarShow = 9,
        VandarStore = 10,
    }

    public enum TransactionType : byte
    {
        IPG = 1,
        DirectDebit = 2
    }

    public enum IPGTransactionStatus : byte
    {
        WaitingForPspResponse = 0,
        FetchingResult = 1,
        SucceededAndWaitingForVerification = 2,
        Failed = 3,
        Expired = 4,
        Verifying = 5,
        VerificationSucceeded = 6,
        VerificationFailed = 7,
        Cancelling = 8,
        CancellationSucceeded = 9,
        CancellationFailed = 10,
    }

    public enum TransactionStatus : byte
    {
        InPrgress = 0,
        TransactionSucceeded = 1,
        TransactionFailed = 2,
        SettlementSucceeded = 3,
        SettlementFailed = 4,
    }

    public enum PaymentStatus
    {
        Draft = 0,
        RedirectedToCpg = 1,
        CanceledByUser = 2,
        InProgress = 3,
        TransactionWaitingForVerification = 4,
        TransactionFailed = 5,
        TransactionVerifiedByApplication = 6,
        TransactionCanceledByApplication = 7,
        TransactionVerificationSucceeded = 8,
        TransactionVerificationFailed = 9,
        TransactionCancellationSucceeded = 10,
        TransactionCancellationFailed = 11,
        SettlementSucceeded = 12,
        SettlementFailed = 13,
    }

    public enum AuditType
    {
        Client = 1,
        Provider = 2,
        User = 3,
        Develop = 4,
    }

    public enum ProviderLogType { }

    public enum ValidityDuration
    {
        OneMonth = 1,
        ThreeMonths = 3,
        SixMonths = 6,
        NineMonths = 9,
        OneYear = 12,
        TwoYears = 24,
        ThreeYears = 36,
        FourYears = 48,
        FiveYears = 60,
    }

    public enum AuthenticationType
    {
        CardInfoCOnfirmation = 1,
        SendOtpCode = 2,
        CheckMobileAndDepositOwnershipMatching = 3
    }
}

public struct ResultData<T>(Enums.OperationResult operationResult)
{
    public T? Data { get; set; } = default(T);

    public Enums.OperationResult OperationResult { get; set; } = operationResult;

    public string? Error { get; set; } = null;
}

