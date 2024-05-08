namespace CPG.API.Helper.Localization;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

public class RouteValueRequestCultureProvider : IRequestCultureProvider
{
    private readonly CultureInfo[] cultures;
    public string DefaultLanguageCode = "fa";

    public RouteValueRequestCultureProvider(CultureInfo[] cultures)
    {
        this.cultures = cultures;
    }

    public Task<ProviderCultureResult> DetermineProviderCultureResult([NotNull] HttpContext httpContext)

    {
        PathString path = httpContext.Request.Path;

        if (string.IsNullOrEmpty(path))
        {
            return Task.FromResult(new ProviderCultureResult(DefaultLanguageCode));
        }

        string[] routeValues = httpContext.Request.Path.Value!.Split('/');
        return routeValues.Length <= 1
            ? Task.FromResult(new ProviderCultureResult(DefaultLanguageCode))
            : !cultures.Any(t => t.Name.Equals(routeValues[1], StringComparison.OrdinalIgnoreCase))
            ? Task.FromResult(new ProviderCultureResult(DefaultLanguageCode))
            : Task.FromResult(new ProviderCultureResult(routeValues[1]));
    }
}
