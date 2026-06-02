using DbLocalizationProvider.Abstractions;

namespace Optimizely.Alloy.Cms13.Resources.Translations;

[LocalizedResource]
public class StartPageResources
{
    [TranslationForNorwegian("Titel")]
    public static string Header { get; set; } = "Page Title";

    [TranslationForNorwegian("Titel 2")]
    [Notes("This resource will be used as subheader for the page.")]
    public static string SubHeader { get; set; } = "Page SubTitle";

    [Notes("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since 1966, when designers at Letraset and James Mosley, the librarian at St Bride Printing Library, took a 1914 Cicero translation and scrambled it to make dummy text for Letraset's Body Type sheets. It has survived not only many decades, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised thanks to these sheets and more recently with desktop publishing software including versions of Lorem Ipsum.")]
    public static string SuperLongNotes { get; set; } = "Long notes";
}
