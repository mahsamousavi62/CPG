using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CharisPayServices.Queries
{
    public class AccountNumberViewModel
    {
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public AccountNumberViewModel(string accountNumber, string bankName)
        {

            AccountNumber = accountNumber;
            BankName = bankName;
        }
    }
}
