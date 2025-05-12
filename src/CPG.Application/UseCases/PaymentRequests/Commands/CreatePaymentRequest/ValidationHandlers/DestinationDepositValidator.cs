using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class DestinationDepositValidator<T> : ValidatorHandler<T>
        where T : DestinationDepositValidatorModel
{
    public override void Handle(T model)
    {
        if (model.DestinationDeposits?.Any() is false && model.Company.CompanyDeposits?.Any() is false && model.Company.CompanyDeposits.All(t => t.IsActive is false))
            throw new PaymentRequestInactiveDepositsException(model.Company.PersianName);

        var activeMethodDeposits = GetActiveMethodsDestinationDeposits(model.PaymentMethodConfig, model.Company, model.ActiveMethods);
        var notExistDeposits = activeMethodDeposits.Where(t => t.Deposits?.Any() is false).Select(t => t.MethodType);
        if (notExistDeposits?.Any() is true)
            throw new PaymentRequestNotExistDepositsException(string.Join(',', notExistDeposits), model.Company.PersianName);

        var inactiveBankMethods = activeMethodDeposits.Where(t => t.Deposits.All(x => x.Bank.IsActive is false)).Select(t => t.MethodType);
        if (model.DestinationDeposits?.Any() is false && inactiveBankMethods?.Any() is true)
            throw new PaymentRequestInactiveDepositBanksException(string.Join(',', inactiveBankMethods), model.Company.PersianName);

    }

    private List<DepositData> GetActiveMethodsDestinationDeposits(PaymentMethodConfig paymentMethodConfig, Company company, List<PaymentMethodType> activeMethods)
    {
        var deposits = new List<DepositData>();

        foreach (var item in activeMethods)
        {
            switch (item)
            {
                case PaymentMethodType.InternetPaymentGateway:
                    {
                        deposits.Add(new DepositData
                        {
                            Deposits = company.CompanyDeposits.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.InternetPaymentGateway)).ToList(),
                            MethodType = PaymentMethodType.InternetPaymentGateway,
                        });
                        break;
                    }
                case PaymentMethodType.PaymentReceipt:
                    {
                        deposits.Add(new DepositData
                        {
                            Deposits = company.CompanyDeposits.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.PaymentReceipt)).ToList(),
                            MethodType = PaymentMethodType.PaymentReceipt,
                        });
                        break;
                    }
                case PaymentMethodType.DirectDebit:
                    {
                        deposits.Add(new DepositData
                        {
                            Deposits = company.CompanyDeposits.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.DirectDebit)).ToList(),
                            MethodType = PaymentMethodType.DirectDebit,
                        });
                        break;
                    }
            }
        }

        return deposits;
    }

    private class DepositData
    {
        public PaymentMethodType MethodType { get; set; }
        public List<CompanyDeposit> Deposits { get; set; }
    }
}
