using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI
{
    public class UiConfigurationContext
    {
        public ICollection<string> AuthorizedAdminRoles { get; } = new DirtyList<string>("Administrators");

        public ICollection<string> AuthorizedEditorRoles { get; } = new DirtyList<string>("Administrators", "Editors");

        public int MaxResourceKeyDisplayLength { get; set; } = 80;

        public ResourceListView DefaultView { get; set; } = ResourceListView.Table;

        public void DisableView(ResourceListView view)
        {
            if(view == ResourceListView.None)
                throw new ArgumentException("Cannot disable `None` view");

            if(view == ResourceListView.Table)
            {
                if(IsTreeViewDisabled)
                    throw new ArgumentException("Cannot disable both views");

                IsTableViewDisabled = true;
            }


            if(view == ResourceListView.Tree)
            {
                if(IsTableViewDisabled)
                    throw new ArgumentException("Cannot disable both views");

                IsTreeViewDisabled = true;
            }

        }

        public bool TreeViewExpandedByDefault { get; set; } = true;

        public bool ShowInvariantCulture { get; set; } = false;

        public static UiConfigurationContext Current { get; } = new UiConfigurationContext();

        internal bool IsTreeViewDisabled { get; set; }

        internal bool IsTableViewDisabled { get; set; }

        public static void Setup(Action<UiConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }
    }
}
