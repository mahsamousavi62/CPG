using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel;

public class Constants
{
    public const string Pattern = "(usr|pwd|merchantConfigurationId|key|iv|userPassword)\\\"\\s*(:)\\s*\"([^\"]*)\"";
    public const string Replaceformat = "$1$2*****";

    public static string CreateCallbackUrl(string callBackUrl, string paymentCode, PaymentStatus paymentStatus)
    {
        return callBackUrl.Contains('?')
            ? $"{callBackUrl.Split("?")[0]}/paymentResult?{callBackUrl.Split("?")[1]}&paymentCode={paymentCode}&paymentStatus={General.GetPaymentStatusTitle(paymentStatus)}"
            : $"{callBackUrl}/paymentResult?paymentCode={paymentCode}&paymentStatus={General.GetPaymentStatusTitle(paymentStatus)}";
    }
}

public static class EnumHelper
{
    public static T ToEnum<T>(this string s) where T : struct
    {
        T newValue;
        return Enum.TryParse(s, out newValue) ? newValue : default(T);
    }
}
