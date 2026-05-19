using DbLocalizationProvider.Abstractions;

namespace Optimizely.Alloy.Cms13.Resources.Translations;

[LocalizedResource]
public class StartPageResources
{
    [TranslationForNorwegian("Titel")]
    public static string Header { get; set; } = "Page Title";
}
