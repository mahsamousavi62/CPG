using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Helper
{
    public static class SharedServices
    {
        public static bool HasProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static DirectDebitGrantStatus GetVandarDirectDebitGrantStatus(string vandarStatus)
        {
            DirectDebitGrantStatus status;
            switch (vandarStatus)
            {
                case "PENDING_VERIFY":
                    {
                        status = DirectDebitGrantStatus.WaitingForConfirmation;
                        break;
                    }
                case "ACTIVE":
                    {
                        status = DirectDebitGrantStatus.Activated;
                        break;
                    }
                case "REVOKED":
                    {
                        status = DirectDebitGrantStatus.Removed;
                        break;
                    }
                case "REVOKED_AUTO":
                    {
                        status = DirectDebitGrantStatus.Removed;
                        break;
                    }
                case "EXPIRED":
                    {
                        status = DirectDebitGrantStatus.Expired;
                        break;
                    }
                default:
                    {
                        status = DirectDebitGrantStatus.Draft;
                        break;
                    }
            }
            return status;
        }

        public static DirectDebitTransactionStatus GetDirectDebitTransactionStatus(string vandarStatus)
        {
            switch (vandarStatus)
            {
                case "PENDING":
                    return DirectDebitTransactionStatus.WaitingForSendToBank;
                case "INIT":
                    return DirectDebitTransactionStatus.WaitingForBankResponse;
                case "DONE":
                    return DirectDebitTransactionStatus.TransactionSucceeded;
                case "FAILED":
                    return DirectDebitTransactionStatus.UnSuccessful;
                case "CANCELED":
                    return DirectDebitTransactionStatus.UnSuccessful;
                case "REVERSED":
                    return DirectDebitTransactionStatus.UnSuccessful;
                default:
                    return DirectDebitTransactionStatus.WaitingForSendToBank;
            }
        }
    }
}