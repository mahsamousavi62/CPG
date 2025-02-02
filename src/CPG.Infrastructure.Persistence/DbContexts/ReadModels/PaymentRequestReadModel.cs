using Azure.Core;
using CPG.Application.Shared;
using CPG.Application.UseCases.PaymentRequests.Queries.Report;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class PaymentRequestReadModel
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ApplicationId { get; set; }
    public string NationalCode { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string CallBackUrl { get; set; }
    public string PaymentCode { get; set; }
    public string TrackerId { get; set; }
    public string PaymentIdentifier { get; set; }
    public Enums.PaymentStatus Status { get; set; }
    public bool IsUsed { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime? VerificationDateTime { get; set; }
    public DateTime UrlExpirationDateTime { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public long CreationUserId { get; set; }
    public ApplicationReadModel Application { get; set; }
    public CompanyReadModel Company { get; set; }
    public TransactionReadModel Transaction { get; set; }
    public List<PaymentRequestMethodReadModel> PaymentRequestMethods { get; set; }

    public static void AddFilter(PaymentFilter paymentFilter,
                                ref  IQueryable<PaymentRequestReadModel> paymentRequests)
    {

        if (!string.IsNullOrWhiteSpace(paymentFilter.PaymentCode))
        {
            paymentRequests = paymentRequests.Where(p =>
               p.PaymentCode == paymentFilter.PaymentCode);
        }
        if (paymentFilter.Amount.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
               p.Amount == paymentFilter.Amount);
        }
        if (!string.IsNullOrEmpty(paymentFilter.NationalCode))
        {
            paymentRequests = paymentRequests.Where(p =>
               p.NationalCode == paymentFilter.NationalCode);
        }
        
        if (paymentFilter.CompanyId.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
              p.CompanyId == paymentFilter.CompanyId);
        }

        if (paymentFilter.Status.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
              p.Status == paymentFilter.Status);
        }
        
        if (paymentFilter.TransactionMethodType.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
              p.Transaction.TransactionMethodType == paymentFilter.TransactionMethodType);
        }

        if (!string.IsNullOrEmpty(paymentFilter.TrackerId))
        {
            paymentRequests = paymentRequests.Where(p =>
             p.TrackerId == paymentFilter.TrackerId);
        }
        if (paymentFilter.FromCreationDate.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
          p.CreationDate.Date >= paymentFilter.FromCreationDate.Value.Date);
        }
        if (paymentFilter.ToCreationDate.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
            p.CreationDate.Date <= paymentFilter.ToCreationDate.Value.Date);
        }

        if (paymentFilter.FromModificationDate.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
            p.ModificationDate.Value.Date >= paymentFilter.FromModificationDate.Value.Date);
        }
        if (paymentFilter.ToModificationDate.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
            p.ModificationDate.Value.Date <= paymentFilter.ToModificationDate.Value.Date);
        }

        if (paymentFilter.FromUrlExpirationDateTime.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
            p.UrlExpirationDateTime.Date >= paymentFilter.FromUrlExpirationDateTime.Value.Date);
        }
        if (paymentFilter.ToUrlExpirationDateTime.HasValue)
        {
            paymentRequests = paymentRequests.Where(p =>
            p.UrlExpirationDateTime.Date <= paymentFilter.ToUrlExpirationDateTime.Value.Date);
        }


    }

    public static void AddSort(PagedFilter pagedFilter,
                                  IQueryable<PaymentRequestReadModel> paymentRequests)
    {
        if (pagedFilter.SortOrder?.ToLower() == "desc")
        {
            paymentRequests = paymentRequests.OrderByDescending(GetSortProperty(pagedFilter));
        }
        else
        {
            paymentRequests = paymentRequests.OrderBy(GetSortProperty(pagedFilter));
        }
    }

    private static Expression<Func<PaymentRequestReadModel, object>> GetSortProperty(PagedFilter pagedFilter) =>
         pagedFilter.SortColumn?.ToLower() switch
          {
              "amount" => product => product.Amount,
              "creationdate" => product => product.CreationDate,
              "modificationdate" => product => product.ModificationDate,
              _ => product => product.Id
          };
}