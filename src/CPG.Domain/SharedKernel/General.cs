
using System;
using CPG.Domain.SharedKernel.Minio;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.AggregateModels.TransactionAggregate;

namespace CPG.Domain.SharedKernel;

public static class General
{
    public static string GetPaymentStatusTitle(PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Draft => "DRAFT",
            PaymentStatus.RedirectedToCpg => "REDIRECTED_TO_CPG",
            PaymentStatus.CanceledByUser => "CANCELLED_BY_USER",
            PaymentStatus.InProgress => "TRANSACTION_IN_PROGRESS",
            PaymentStatus.TransactionWaitingForVerification => "TRANSACTION_WAITING_FOR_VERIFICATION",
            PaymentStatus.TransactionFailed => "TRANSACTION_FAILED",
            PaymentStatus.TransactionVerifiedByApplication => "TRANSACTION_VERIFIED_BY_APPLICATION",
            PaymentStatus.TransactionCanceledByApplication => "TRANSACTION_CANCELLED_BY_APPLICATION",
            PaymentStatus.TransactionVerificationSucceeded => "TRANSACTION_VERIFICATION_SUCCEEDED",
            PaymentStatus.TransactionVerificationFailed => "TRANSACTION_VERIFICATION_FAILED",
            PaymentStatus.TransactionCancellationSucceeded => "TRANSACTION_CANCELLATION_SUCCEEDED",
            PaymentStatus.TransactionCancellationFailed => "TRANSACTION_CANCELLATION_FAILED",
            PaymentStatus.SettlementSucceeded => "SETTLEMENT_SUCCEEDED",
            PaymentStatus.SettlementFailed => "SETTLEMENT_FAILED",
            _ => string.Empty
        };
    }

    public static string GetTransactionStatusName(Enums.TransactionStatus status)
    {
        return status switch
        {
            Enums.TransactionStatus.InPrgress =>Resource.InPrgress,
            Enums.TransactionStatus.TransactionSucceeded => Resource.TransactionSucceeded,
            Enums.TransactionStatus.TransactionFailed => Resource.TransactionFailed,
            _ => string.Empty,
        };
    }

    public static string GetIPGTransactionStatusName(IPGTransactionStatus status)
    {
        return status switch
        {
            IPGTransactionStatus.WaitingForPspResponse => Resource.WaitingForPspResponse,
            IPGTransactionStatus.FetchingResult => Resource.FetchingResult,
            IPGTransactionStatus.SucceededAndWaitingForVerification => Resource.SucceededAndWaitingForVerification,
            IPGTransactionStatus.Failed => Resource.Failed,
            IPGTransactionStatus.Expired => Resource.Expired,
            IPGTransactionStatus.Verifying => Resource.Verifying,
            IPGTransactionStatus.VerificationSucceeded => Resource.VerificationSucceeded,
            IPGTransactionStatus.VerificationFailed => Resource.VerificationFailed,
            IPGTransactionStatus.WaitingForSettlementRequest => Resource.WaitingForSettlementRequest,
            IPGTransactionStatus.SettlementSucceeded => Resource.SettlementSucceeded,
            IPGTransactionStatus.SettlementFailed => Resource.SettlementFailed,
            IPGTransactionStatus.Cancelling => "Cancelling transaction",
            IPGTransactionStatus.CancellationSucceeded => "Transaction cancellation succeeded",
            IPGTransactionStatus.CancellationFailed => "Transaction cancellation failed",
            _ => string.Empty,
        };
    }

    public static string GetPaymentReceiptTransactionStatusName(PaymentReceiptStatus status)
    {
        return status switch
        {
            PaymentReceiptStatus.WaitingForResponseFromProvider => Resource.WaitingForResponseFromProvider,
            PaymentReceiptStatus.SucceededAndWaitingForVerification => Resource.SucceededAndWaitingForVerification,
            PaymentReceiptStatus.Failed =>Resource.Failed,
            _ => string.Empty,
        };
    }

    public static string GetCharismaCardTransactionStatusName(CharismaCardStatus status)
    {
        return status switch
        {
            CharismaCardStatus.Done => Resource.Done,
            CharismaCardStatus.Failed => Resource.Failed,
            _ => string.Empty,
        };
    }
   
    public static async Task<string> GetLogo(IMinioProvider minioProvider, string logoPath)
    {
        try
        {
            return await minioProvider.PresignedGetObject(logoPath);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string GetTransactionMethodTypeName(Enums.TransactionType transactionMethodType)
    {
        switch (transactionMethodType)
        {
            case Enums.TransactionType.IPG:
                return Resource.IPG;
            case Enums.TransactionType.DirectDebit:
                return Resource.DirectDebit;
            case Enums.TransactionType.PaymentReceipt:
                return Resource.PaymentReceipt;
            case Enums.TransactionType.CharismaCard:
                return Resource.CharismaCard;
            default:
                return string.Empty;
        }
    }
    public static string GetPaymentMethodTypeTitle(TransactionType type)
    {
        return type switch
        {
            TransactionType.IPG => "INTERNET_PAYMENT_GATEWAY",
            TransactionType.DirectDebit => "DIRECT_DEBIT",
            TransactionType.PaymentReceipt => "PAYMENT_RECEIPT",
            TransactionType.CharismaCard => "CHARISMA_CARD",
            _ => string.Empty
        };
    }

    public static string GetTransactionRefrenceNumber(Transaction transaction)
    {
        switch (transaction.TransactionMethodType)
        {
            case TransactionType.IPG:
                return transaction.IPGTransaction.ReferenceNumber;
            case TransactionType.DirectDebit:
                return string.Empty;
            case TransactionType.PaymentReceipt:
                return transaction.PaymentReceiptTransaction.ReferenceNumber;
            case TransactionType.CharismaCard:
                return transaction.CharismaCardTransaction.ReferenceNumber;
            default:
                return string.Empty;
        }
    }

    public static string GetTransactionVerificationDateTime(Transaction transaction)
    {
        switch (transaction.TransactionMethodType)
        {
            case TransactionType.IPG:
                return transaction.IPGTransaction.VerificationDateTime?.ToString("yyyy-MM-dd HH:mm:ss zzz");
            case TransactionType.DirectDebit:
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            case TransactionType.PaymentReceipt:
            case TransactionType.CharismaCard:
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");

            default:
                return string.Empty;
        }
    }
}
