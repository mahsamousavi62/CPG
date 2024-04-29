using System;
using System.Collections.Generic;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.Companies.ViewModels;

public class CompanyViewModel
{
    public long Id { get; set; }

    public string PersianName { get; set; }

    public string EnglishName { get; set; }

    public bool NationalCodeMatchingRequied { get; set; }

    public string SiteAddress { get; set; }

    public short IpgRedirectionMethodType { get; set; }

    public string Logo { get; set; }

    public short? Code { get; set; }

    public ICollection<Enums.PaymentMethodType> PaymentMethods { get; set; }

    public ICollection<UserCompanyViewModel> Users { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ModificationDate { get; set; }
}
