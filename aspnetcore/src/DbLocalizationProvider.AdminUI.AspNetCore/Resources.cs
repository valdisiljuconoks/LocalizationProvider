// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

[LocalizedResource]
[Hidden]
public class Resources
{
    public static string Title = "Admin UI";
    public static string Header = "Localization Resource Editor";
    public static string Add = "Add";
    public static string Export = "Export";
    public static string Import = "Import";
    public static string Validate = "Validate";
    public static string TableView = "Table View";
    public static string TreeView = "Tree View";
    public static string Languages = "Languages";
    public static string Settings = "Settings";
    public static string ShowHiddenResources = "Show Hidden Resources";
    public static string Save = "Ok, Save!";
    public static string Cancel = "Cancel";
    public static string Remove = "Remove";
    public static string SelectAll = "(Select All)";
    public static string RemoveConfirmation = "Do you want to remove this translation?";
    public static string SearchPlaceholder = "if it gets too noisy, type filter here...";
    public static string SearchPlaceholderDbSearchEnabled = "server side search enabled, type resource key...";
    public static string ResourceKeyColumn = "Key";
    public static string InvariantCultureColumn = "Invariant";
    public static string HiddenColumn = "Is Hidden?";
    public static string FromCodeColumn = "From code?";
    public static string DeleteColumn = "Delete?";
    public static string DeleteConfirmation = "Do you want to throw out this resource?";
    public static string ShowOnlyEmptyResources = "Show Only Empty Resources";
    public static string EmptyTranslation = "Empty";
    public static string CleanCache = "Clean Cache";
    public static string CleanCacheConfirmation = "Wanna start with clean cache state?";
    public static string ErrorLoadingResources = "There was an error while loading resources. Please check logs for more details!";
    public static string AvailableLanguages = "Available Languages";
    public static string XliffLanguages = "Select XLIFF Export Languages";
    public static string XliffLanguagesSource = "Source Language";
    public static string XliffLanguagesTarget = "Target Language";
    public static string ResourceKey = "Resource Key";
    public static string Translation = "Translation";
    public static string ImportViewTitle = "Import Localization Resources";
    public static string InsertsLabel = "Inserts";
    public static string UpdatesLabel = "Updates";
    public static string DeletesLabel = "Deletes";
    public static string ImportChooseActionLabel = "Choose";
    public static string ImportOperationLabel = "Operation";
    public static string Close = "Close";
    public static string Loading = "Loading data...";
    public static string LoadAll = "Load All Resources";
}
