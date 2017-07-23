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
            ConfigurationContext.Setup(cfg =>
                                       {
                                           cfg.DiagnosticsEnabled = true;
                                           cfg.ModelMetadataProviders.EnableLegacyMode = () => true;
                                           cfg.CustomAttributes = new[]
                                                                  {
                                                                      new CustomAttributeDescriptor(typeof(HelpTextAttribute), false)
                                                                  };

                                           cfg.ForeignResources.Add(typeof(VersionStatus));

                                           cfg.CacheManager.OnRemove += CacheManagerOnOnRemove;
                                       });

            UiConfigurationContext.Setup(cfg =>
                                         {
                                             cfg.DefaultView = ResourceListView.Tree;
                                             cfg.TreeViewExpandedByDefault = true;
                                         });
        }

        public void Uninitialize(InitializationEngine context)
        {
            ConfigurationContext.Current.CacheManager.OnRemove -= CacheManagerOnOnRemove;
        }

        private void CacheManagerOnOnRemove(CacheEventArgs cacheEventArgs)
        {
            var z = 0;
        }
    }
}
