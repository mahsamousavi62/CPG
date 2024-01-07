namespace CPG.Domain.SharedKernel.Helper;

public interface IUserAgentService
{
     string GetUserAgent();
    string GetClientIPAddress();
}
