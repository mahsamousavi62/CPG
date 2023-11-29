using System;

namespace CPG.Application.UseCases.Banks.ViewModels;

public class BankViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public string IbanPrefix { get; set; }

    public string LogoAddress { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ModificationDate { get; set; }
}
