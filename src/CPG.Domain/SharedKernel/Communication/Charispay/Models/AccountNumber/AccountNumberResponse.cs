using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber
{
    public class AccountNumberResponse
    {
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public AccountNumberResponse(string accountNumber, string bankName)
        {

            AccountNumber = accountNumber;
            BankName = bankName;
        }
    }
}
