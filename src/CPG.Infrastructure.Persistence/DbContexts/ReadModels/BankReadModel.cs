using CPG.Domain.AggregateModels.BankAggregate;
using System;
using System.Collections.Generic;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class BankReadModel
{
    private string _logoAddress;
    public string LogoAddress
    {
        get => _logoAddress;
        set
        {
            _logoAddress = value;

            Logo = _logoAddress;
        }
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string IbanPrefix { get; set; }
    public bool IsActive { get; set; }

    public bool? HasDirectDebitFeature { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public ICollection<CompanyDepositReadModel> CompanyDeposits { get; set; }
    public ICollection<DirectDebitGrantReadModel> DirectDebitGrants { get; set; }
    public BankDirectDebitSettingReadModel? DirectDebitSetting { get; set; }
    public string Logo { get; private set; }
}
