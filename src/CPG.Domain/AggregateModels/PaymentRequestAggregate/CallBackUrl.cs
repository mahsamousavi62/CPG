using Ardalis.GuardClauses;
using System;
using System.Text.RegularExpressions;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate
{
    public class CallBackUrl
    {
        public string Value { get; }

        private CallBackUrl()
        {
        }

        public CallBackUrl(string callbackUrl)
        {
            Guard.Against.NullOrWhiteSpace(callbackUrl, nameof(callbackUrl));

            const string pattern = @"^(https?|http?):\/\/[^\s\/$.?#].[^\s]*$";
            if (!Regex.IsMatch(callbackUrl, pattern))
                throw new Exception();
        }

        public static implicit operator string(CallBackUrl callbackUrl) => callbackUrl.Value;
        public static implicit operator CallBackUrl(string value) => new(value);
        public override string ToString() => Value;
    }
}
