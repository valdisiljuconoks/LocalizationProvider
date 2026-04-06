using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;

namespace DbLocalizationProvider.Optimizely.Sandbox.Business.EditorDescriptors;

/// <summary>
/// Provides a list of options corresponding to ContactPage pages on the site
/// </summary>
/// <seealso cref="ContactPageSelector"/>
[ServiceConfiguration]
public class ContactPageSelectionFactory(ContentLocator contentLocator) : ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var contactPages = contentLocator.GetContactPages();

        return new List<SelectItem>(contactPages.Select(c => new SelectItem { Value = c.ContentLink, Text = c.Name }));
    }
}
