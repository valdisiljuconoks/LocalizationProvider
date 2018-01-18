using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.AdminUI
{
    /// <summary>
    /// Main class responsible for providing way to customize AdminUI
    /// </summary>
    public class UiConfigurationContext
    {
        /// <summary>
        /// Set roles to users who will have admin access to UI (can delete resources, etc).
        /// </summary>
        public ICollection<string> AuthorizedAdminRoles { get; } = new DirtyList<string>("Administrators");

        /// <summary>
        /// Set roles to users who will have editor access to UI (can add translations).
        /// </summary>
        public ICollection<string> AuthorizedEditorRoles { get; } = new DirtyList<string>("Administrators", "Editors");

        /// <summary>
        /// Sometimes resource keys might get pretty long.
        /// </summary>
        public int MaxResourceKeyDisplayLength { get; set; } = 80;

        /// <summary>
        /// Someone asked me once - "I like tree view, can it be my default preference".
        /// </summary>
        public ResourceListView DefaultView { get; set; } = ResourceListView.Table;

        /// <summary>
        /// If you wanna get rid of some view (table OR tree) this is the method. You cannot disable all views - will receive exception.
        /// </summary>
        /// <param name="view"></param>
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

        /// <summary>
        /// Tree view will be expanded by default.
        /// </summary>
        public bool TreeViewExpandedByDefault { get; set; } = true;

        /// <summary>
        /// This is sometimes pretty useful when want to see what exactly resource translation was synced from code.
        /// </summary>
        public bool ShowInvariantCulture { get; set; } = false;

        /// <summary>
        /// Access to current configuration context instance. Statics sucks.
        /// </summary>
        public static UiConfigurationContext Current { get; } = new UiConfigurationContext();

        internal bool IsTreeViewDisabled { get; set; }

        internal bool IsTableViewDisabled { get; set; }

        /// <summary>
        /// Wanna customize anything here? Call this method.
        /// </summary>
        /// <param name="configCallback"></param>
        public static void Setup(Action<UiConfigurationContext> configCallback)
        {
            configCallback?.Invoke(Current);
        }
    }
}
