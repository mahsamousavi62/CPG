
using System;
using static CPG.Domain.SharedKernel.Enums;

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
            Enums.TransactionStatus.InPrgress => "در حال انجام",
            Enums.TransactionStatus.TransactionSucceeded => "تراکنش موفق",
            Enums.TransactionStatus.TransactionFailed => "تراکنش ناموفق",
            _ => string.Empty,
        };
    }


    public static string GetIPGTransactionStatusName(IPGTransactionStatus status)
    {
        return status switch
        {
            IPGTransactionStatus.WaitingForPspResponse => "در انتظار پاسخ از درگاه",
            IPGTransactionStatus.FetchingResult => "در انتظار دریافت اطلاعات تراکنش",
            IPGTransactionStatus.SucceededAndWaitingForVerification => "موفق و در انتظار تایید",
            IPGTransactionStatus.Failed => "ناموفق",
            IPGTransactionStatus.Expired => "منقضی شده",
            IPGTransactionStatus.Verifying => "در انتظار تایید تراکنش در درگاه",
            IPGTransactionStatus.VerificationSucceeded => " تایید موفق در درگاه",
            IPGTransactionStatus.VerificationFailed => "تایید ناموفق در درگاه",
            IPGTransactionStatus.WaitingForSettlementRequest => "در انتظار ثبت درخواست تسویه در درگاه",
            IPGTransactionStatus.SettlementSucceeded => "ثبت درخواست تسویه موفق",
            IPGTransactionStatus.SettlementFailed => "ثبت درخواست تسویه ناموفق",
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
            PaymentReceiptStatus.WaitingForResponseFromProvider => "در انتظار پاسخ از سرویس دهنده",
            PaymentReceiptStatus.SucceededAndWaitingForVerification => "موفق و در انتظار تایید",
            PaymentReceiptStatus.Failed => "ناموفق",
            _ => string.Empty,
        };
    }
}
