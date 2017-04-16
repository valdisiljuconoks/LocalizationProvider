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
        public static string Export => "Export";
        public static string Import => "Import";
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
    }
}
