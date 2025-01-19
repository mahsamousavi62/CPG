namespace CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;


public class PrivatePerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class LegalPerson
{
    public string CompanyName { get; set; }
}

public class Result
{
    public string Id { get; set; }
    public string UniqueIdentifier { get; set; }
    public long? Mobile { get; set; }
    public bool IsLegal { get; set; }
    public PrivatePerson PrivatePerson { get; set; }
    public LegalPerson LegalPerson { get; set; }
}

public class UserProfileResponse : IHttpResponse
{
    public Result Result { get; set; }
    public short StatusCode { get; set; }
}



public class IdpProfileRequest : IHttpRequest
{
    public string IdpId { get; set; }
}

public interface IHttpRequest
{
}

public interface IHttpResponse
{
    public short StatusCode { get; set; }
}

