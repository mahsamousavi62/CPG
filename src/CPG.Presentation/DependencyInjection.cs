using Microsoft.Extensions.DependencyInjection;

namespace CPG.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
            => services;
    }
}