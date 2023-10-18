using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.Commands.UpdateBank
{
    public record UpdateBankCommand(int id, string name, string swiftCode, BankStatus status, byte[] logo,
                                    int providerId, string providerData, decimal directDebitAmountLimit,
                                    decimal directDebitDailyTransactionLimit, long cpgUserId) : IRequest<int>;
}
