using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel
{
    public class Enums
    {
        public enum BankStatus
        {
            Active = 1,
            Inactive = 2,
            Suspended = 3,
        }

        public enum ApplicationSettingEntityType
        {
            IDPCredential = 1,
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

        public enum CompanyPaymentMethod
        {
            InternetPaymentGateway = 1,
            DirectDebit = 2,
            PaymentReceipt = 3
        }
    }

    public struct ResultData<T>
    {
        public T? Data { get; set; }

        public Enums.OperationResult OperationResult { get; set; }

        public string? Error { get; set; }

        public ResultData(Enums.OperationResult operationResult)
        {
            Data = default(T);
            Error = null;
            OperationResult = operationResult;
        }


    }
}
