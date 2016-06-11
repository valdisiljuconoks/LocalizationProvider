using System.Collections.Generic;
using System.Linq;
using EPiServer.PlugIn;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using PlugInArea = EPiServer.PlugIn.PlugInArea;

namespace DbLocalizationProvider.AdminUI.EPiServer
{
    [GuiPlugIn(DisplayName = "Localization Resources", UrlFromModuleFolder = "LocalizationResources", Area = PlugInArea.AdminMenu)]
    public class MenuItemRegistration { }

    [MenuProvider]
    public class AdminUIMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            return new List<MenuItem>
                   {
                       new UrlMenuItem("Localization", "/global/cms/localization", Paths.ToResource(typeof(MenuItemRegistration), "LocalizationResources/Main"))
                       {
                           IsAvailable = ctx => UiConfigurationContext.Current.AuthorizedEditorRoles.Any(ctx.HttpContext.User.IsInRole)
                       }
                   };
        }
    }
}
