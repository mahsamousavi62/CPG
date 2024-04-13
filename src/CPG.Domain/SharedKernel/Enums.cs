using System;
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

    public enum NeoBankDepositStatus : byte
    {
        NotCustomer = 0,
        NoDeposite = 1,
        PendingActivation = 2,
        Active = 3,
        DeActive = 4,
    }

    public enum NeoBankTransferType
    {
        Internal = 0,
        External = 1,
    }

    public enum NeoBankTransferStatus
    {
        Failed = 0,
        Done = 1
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
        PaymentReceipt = 3,
        CharismaCard = 4,
    }

    public enum UploadFromEntityType
    {
        Company = 1,
        Bank = 2,
        Provider = 3,
        Application = 4,
        PaymentReceipt = 5,
    }

    public enum ProviderType : byte
    {
        Vandar = 1,
        AsanPardakht = 2,
        Sep = 3,
        Pec = 4,
        BehPardakht = 5,
        Ayandeh = 6,
        Idp = 101,
        NeoBank = 102,
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
        VandarVerify = 11,
        GetIdpToken = 12,
        GetIdpProfile = 13,
        GetUserDepositBalance = 14,
        ClientDirectDebit = 15,
        BehPardakhtToken = 16,
        BehPardakhtTransResult = 17,
        BehPardakhtVerify = 18,
        AyandehToken = 19,
        AyandehTransResult = 20,
        AyandehVerify = 21,
        GetAccountNumber=22,
    }

    public enum TransactionType : byte
    {
        IPG = 1,
        DirectDebit = 2,
        PaymentReceipt = 3,
        CharismaCard = 4,
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
        WaitingForSettlementRequest = 8,
        SettlementSucceeded = 9,
        SettlementFailed = 10,
        Cancelling = 11,
        CancellationSucceeded = 12,
        CancellationFailed = 13,
    }

    public enum TransactionStatus : byte
    {
        InPrgress = 0,
        TransactionSucceeded = 1,
        TransactionFailed = 2,
        SettlementSucceeded = 3,
        SettlementFailed = 4,
    }

    public enum DirectDebitTransactionStatus : byte
    {
        WaitingForSendToBank = 0,
        WaitingForBankResponse = 1,
        TransactionSucceeded = 2,
        UnSuccessful = 3,
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
        CardInfoConfirmation = 1,
        SendOtpCode = 2,
        CheckMobileAndDepositOwnershipMatching = 3
    }

    public enum DirectDebitGrantStatus
    {
        Draft = 0,
        WaitingForConfirmation = 1,
        Activated = 2,
        Voided = 3,
        CanceledByUser = 4,
        Removed = 5,
        Expired = 6,
    }

    public enum PaymentReceiptStatus : byte
    {
        WaitingForResponseFromProvider = 0,
        SucceededAndWaitingForVerification = 1,
        Failed = 2
    }

    public enum CharismaCardStatus : byte
    {
        Failed = 0,
        Done = 1
    }
}
public static class EnumExtensions
{
    public static string GetName<T>(this T enumValue) where T : Enum
    {
        return Enum.GetName(typeof(T), enumValue)!;
    }

    public static string GetValue<T>(this T enumValue) where T : Enum
    {
        return enumValue.ToString("D");
    }
}
public struct ResultData<T>(Enums.OperationResult operationResult)
{
    public T? Data { get; set; } = default(T);

    public Enums.OperationResult OperationResult { get; set; } = operationResult;

    public string? Error { get; set; } = null;
}

