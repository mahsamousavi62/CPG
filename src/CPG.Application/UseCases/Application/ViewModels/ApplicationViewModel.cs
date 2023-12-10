using System;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Application.ViewModels;

public class ApplicationViewModel
{
    public long Id { get; set; }

    public string PersianName { get; set; }

    public string EnglishName { get; set; }

    public string Logo { get; set; }

    public string ResponseApiUrl { get; set; }

    public Dictionary<long, string> IdpClientIds { get; set; }

    public Dictionary<long, string> CallbackUrls { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ModificationDate { get; set; }
}
