using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

public class RequiredDirectDebitSettingException(int bankId) : DomainException(Resource.RequiredDirectDebitSetting)
{
    public override string Code => "required_directDebit_setting";        
}