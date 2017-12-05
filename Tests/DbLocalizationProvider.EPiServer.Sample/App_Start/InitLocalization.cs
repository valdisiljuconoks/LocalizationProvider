using DbLocalizationProvider.AdminUI;
using DbLocalizationProvider.Cache;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.EPiServer.Sample
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class InitLocalization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            ConfigurationContext.Setup(_ =>
            {
                _.DiagnosticsEnabled = true;
                _.ModelMetadataProviders.EnableLegacyMode = () => true;
                _.CustomAttributes = new[]
                {
                    new CustomAttributeDescriptor(typeof(HelpTextAttribute), false)
                };

                _.ForeignResources.Add(typeof(VersionStatus));

                _.CacheManager.OnRemove += CacheManagerOnOnRemove;
            });

            UiConfigurationContext.Setup(_ =>
            {
                _.DefaultView = ResourceListView.Tree;
                _.TreeViewExpandedByDefault = true;
                _.ShowInvariantCulture = true;
            });
        }

        public void Uninitialize(InitializationEngine context)
        {
            ConfigurationContext.Current.CacheManager.OnRemove -= CacheManagerOnOnRemove;
        }

        private void CacheManagerOnOnRemove(CacheEventArgs cacheEventArgs)
        {
        }
    }
}
