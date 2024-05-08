using CPG.API.Helper.Localization;

namespace CPG.API.Helper.Localization;

using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

public static class LocalizationExtensions
{
    public const string DefaultLanguageCode = "fa";

    public static RequestLocalizationOptions RequestLocalizationOptions
    {
        get
        {
            var supportedCultures = new CultureInfo[]
        {
            new CultureInfo("en"),
            new CultureInfo("fa")
        };
            return new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(DefaultLanguageCode),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = [new RouteValueRequestCultureProvider(supportedCultures)],
            };
        }
    }
}