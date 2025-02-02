using Mapster;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels;

public class CreatePaymentRequestViewModel : IRegister
{
    [Required]
    public short CompanyCode { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string CallBackUrl { get; set; }

    [Required]
    public string NationalCode { get; set; }

    [Required]
    public string TrackerId { get; set; }

    public string PaymentIdentifier { get; set; }

    public string Description { get; set; }

    [AllowNull]
    public PaymentMethodConfig PaymentMethodConfig { get; set; }

    public bool IsAnonymous { get; set; } = false;

    public void Register(TypeAdapterConfig config)
    {
        config.ForType<CreatePaymentRequestViewModel, PaymentRequest>();
    }
}

public class PaymentMethodConfig
{
    public IpgConfig IpgConfig { get; set; }
    public DirectDebitConfig DirectDebitConfig { get; set; }
    public PaymentReceiptConfig PaymentReceiptConfig { get; set; }
    public CharismaCardConfig CharismaCardConfig { get; set; }
}

public class MethodConfigBase
{
    [Required]
    public bool IsActive { get; set; }    
}

public class CharismaCardConfig : MethodConfigBase;

public class DirectDebitConfig : MethodConfigBase
{
    public string DestinationDepositIban { get; set; }
}

public class IpgConfig : MethodConfigBase
{
    public List<string> DestinationDepositIban { get; set; }
    public List<short> IpgTypeCode { get; set; }
}

public class PaymentReceiptConfig : MethodConfigBase
{
    public List<string> DestinationDepositIban { get; set; }
}
