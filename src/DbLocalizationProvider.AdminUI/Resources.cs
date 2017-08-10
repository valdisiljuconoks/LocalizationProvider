using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.AdminUI
{
    [LocalizedResource]
    [Hidden]
    public class Resources
    {
        public static string Header => "Localization Resources";
        public static string AvailableLanguages => "Available Languages";
        public static string Save => "Save";
        public static string Back => "Back";
        public static string Export => "Export";
        public static string SearchPlaceholder => "Enter Search Query";
        public static string New => "New Resource";
        public static string ShowEmpty => "Show Empty Resources";
        public static string ShowHidden => "Show Hidden Resources";
        public static string KeyColumn => "Resource Key";
        public static string DeleteColumn => "Delete";
        public static string FromCodeColumn => "From Code";
        public static string TranslationPopupHeader => "Enter translation";
        public static string Empty => "Empty";
        public static string DeleteConfirm => "Do you really want to delete";
        public static string TreeView => "Tree View";
        public static string TableView => "Table View";
        public static string ExpandAll => "Expand All";
        public static string CollapseAll => "Collapse All";
        public static string ChooseLanguage => "Choose Language";
        public static string Close => "Close";

        [LocalizedResource]
        [Hidden]
        public class ImportResources
        {
            public static string Import => "Import";
            public static string Current => "Current";
            public static string ImportPreview => "Preview import (do not commit)";
            public static string ImportHeader => "Import Localization Resources";
            public static string ImportCommitReview => "Please review your pending changes:";
            public static string ImportIntro => "Import localization resources exported from other EPiServer or translation application.";
            public static string SourceLanguage => "Source Language";
            public static string TargetLanguage => "Target Language";
            public static string SelectFile => "Select file to upload";
            public static string Inserts => "Inserts:";
            public static string Updates => "Updates:";
            public static string Deletes => "Deletes:";
            public static string Choose => "Choose";
            public static string Operation => "Operation";
            public static string NoChanges => "You all good (or either bad), no changes detected!";
        }
    }
}
