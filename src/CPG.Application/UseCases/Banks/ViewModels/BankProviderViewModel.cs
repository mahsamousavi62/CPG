using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.ViewModels;

public class BankProviderViewModel
{
    public long Id { get; set; }
    public int BankId { get; set; }
    public ProviderType ProviderType { get; set; }
    public string BankName { get; set; }
}
