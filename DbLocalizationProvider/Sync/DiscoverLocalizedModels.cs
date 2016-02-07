using System;
using System.Linq;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.Sync
{
    [InitializableModule]
    [ModuleDependency(typeof (InitializationModule))]
    public class DiscoverLocalizedModels : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            if (!ConfigurationContext.Current.DiscoverAndRegisterLocalizedModels())
            {
                return;
            }

            var types = TypeDiscoveryHelper.GetTypesOfInterface<ILocalizedModel>();

            using (var db = new LanguageEntities("EPiServerDB"))
            {
                foreach (var type in types)
                {
                    var properties = TypeDiscoveryHelper.GetAllProperties(type);

                    foreach (var property in properties)
                    {
                        var info = property.Item1;
                        var resourceKey = property.Item2;
                        var resourceValue = resourceKey;

                        if (TypeDiscoveryHelper.IsStaticStringProperty(info))
                        {
                            try
                            {
                                resourceValue = info.GetGetMethod().Invoke(null, null) as string;
                            }
                            catch { }
                        }

                        var existingResource = db.LocalizationResources.FirstOrDefault(r => r.ResourceKey == resourceKey);

                        if (existingResource != null)
                        {
                            continue;
                        }

                        // create new resource
                        var resource = new LocalizationResource(resourceKey)
                                       {
                                           ModificationDate = DateTime.UtcNow,
                                           Author = "type-scanner"
                                       };

                        var translation = new LocalizationResourceTranslation
                        {
                            Language = ContentLanguage.PreferredCulture != null ? ContentLanguage.PreferredCulture.Name : "en",
                            Value = resourceValue
                        };

                        resource.Translations.Add(translation);

                        db.LocalizationResources.Add(resource);
                        db.SaveChanges();
                    }
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}
