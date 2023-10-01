using System.Collections;
using System.Collections.Generic;
using System.Resources;
using CPG.Domain;

namespace CPG.Application.Shared.Resource
{
    public class ResourceHandler : IResourceHandler
    {
        private readonly ResourceManager resourceManager;
        private const string resourceFullyQualifiedName = "CPG.Application.Shared.Resource.GlobalResource";


        public ResourceHandler()
        {
            resourceManager = new ResourceManager(resourceFullyQualifiedName, typeof(GlobalResource).Assembly);
        }
        public Dictionary<string, string> GetResources()
        {
            var resourceDictionary = new Dictionary<string, string>();

            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("fa");

            string[] resourceNames = typeof(GlobalResource).Assembly.GetManifestResourceNames();

            var resourceSet = resourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

            if (resourceSet == null)
            {
                return null;
            }

            foreach (DictionaryEntry resource in resourceSet)
            {
                var key = resource.Key.ToString();
                var value = resource.Value?.ToString();

                resourceDictionary[key ?? string.Empty] = value ?? string.Empty;
            }

            return resourceDictionary;
        }
    }
}
