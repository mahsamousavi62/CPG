using System.Collections.Generic;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;

public class UserGrantsResponse : ResponseBase
{
    public int GrantStatus { get; set; }
    public string GrantMessage { get; set; }
    public List<GrantData> Grants { get; set; }
}
