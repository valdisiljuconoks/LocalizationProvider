using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace DbLocalizationProvider.AdminUI.EPiServer
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class AdminUISetupModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            foreach (var role in new[] { "CmsAdmins", "WebAdmins", "LocalizationAdmins" })
                UiConfigurationContext.Current.AuthorizedAdminRoles.Add(role);

            foreach (var role in new[] { "CmsEditors", "WebEditors", "LocalizationEditors" })
                UiConfigurationContext.Current.AuthorizedEditorRoles.Add(role);

            // set default implementations
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}
