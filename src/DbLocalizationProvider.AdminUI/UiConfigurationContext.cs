using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI
{
    public class UiConfigurationContext
    {
        public ICollection<string> AuthorizedAdminRoles { get; } = new List<string> { "Administrators" };

        public ICollection<string> AuthorizedEditorRoles { get; } = new List<string> { "Administrators", "Editors" };

        public int MaxResourceKeyDisplayLength { get; set; } = 80;

        public static UiConfigurationContext Current { get; } = new UiConfigurationContext();
    }
}
