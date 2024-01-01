using System;

namespace CPG.Application.UseCases.IPGTypes.ViewModels;

public class IPGTypeViewModel
{
    public long Id { get; set; }
    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public string Logo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
}
