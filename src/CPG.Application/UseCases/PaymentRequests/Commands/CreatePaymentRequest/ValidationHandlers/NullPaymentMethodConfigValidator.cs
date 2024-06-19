using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers
{
    internal class NullPaymentMethodConfigValidator<T> : ValidatorHandler<T>
        where T : Company
    {
        private readonly string MiddleEastIbanPrefix = "078";

        public override void Handle(T company)
        {
            if (company.CompanyDeposits?.Any(t => t.IsActive) is false)
                throw new PaymentRequestNoActiveCompanyDepositFoundException(company.PersianName);

            if (company.CompanyDeposits?.Any(t => t.Bank.IsActive) is false)
                throw new PaymentRequestNoActiveDepositBankException(company.PersianName);

            if (company.CompanyDeposits?.Any(t => t.PaymentMethods != null && t.PaymentMethods.Count > 0) is false)
                throw new PaymentRequestNoDepositPaymentMethodException(company.PersianName);

            var companyIpgs = company.CompanyIPGs.Where(t => t.IsActive &&
                                                             t.Provider.IsActive &&
                                                             t.Provider.PaymentMethods != null &&
                                                             t.Provider.PaymentMethods.Select(x => x.MethodType)
                                                                                      .Contains(PaymentMethodType.InternetPaymentGateway));

            var ipgDeposits = company.CompanyDeposits.Where(t => t.IsActive &&
                                                                 t.Bank.IsActive &&
                                                                 t.PaymentMethods != null &&
                                                                 t.PaymentMethods.Select(x => x.MethodType)
                                                                                 .Contains(PaymentMethodType.InternetPaymentGateway) &&
                                                                 companyIpgs.SelectMany(x => x.IPGDeposits.Where(q => q.IsDefault)
                                                                                                          .Select(q => q.CompanyDepositId))
                                                                            .Contains(t.Id));

            var ddDeposits = company.CompanyDeposits.Where(t => t.IsActive &&
                                                                t.Bank.IsActive &&
                                                                t.IsDefaultForDirectDebit is true &&
                                                                t.PaymentMethods != null &&
                                                                t.PaymentMethods.Select(x => x.MethodType)
                                                                                .Contains(PaymentMethodType.DirectDebit));

            var receiptDeposits = company.CompanyDeposits.Where(t => t.IsActive &&
                                                                     t.Bank.IsActive &&
                                                                     t.PaymentMethods != null &&
                                                                     t.PaymentMethods.Select(x => x.MethodType)
                                                                                     .Contains(PaymentMethodType.PaymentReceipt));

            var cardDeposits = company.CompanyDeposits.Where(t => t.IsActive &&
                                                                  t.Bank.IsActive &&
                                                                  t.Bank.IbanPrefix == MiddleEastIbanPrefix &&
                                                                  t.IsDefaultForCharismaCard is true &&
                                                                  t.PaymentMethods != null &&
                                                                  t.PaymentMethods.Select(x => x.MethodType)
                                                                                  .Contains(PaymentMethodType.CharismaCard));

            if (ipgDeposits?.Any() is false && ddDeposits?.Any() is false && receiptDeposits?.Any() is false && cardDeposits?.Any() is false)
                throw new PaymentRequestNoUsableMethodsException(company.PersianName);
        }
    }
}
