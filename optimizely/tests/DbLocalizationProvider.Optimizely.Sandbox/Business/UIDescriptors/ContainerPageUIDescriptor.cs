using DbLocalizationProvider.Optimizely.Sandbox.Models.Pages;
using EPiServer.Shell;

namespace DbLocalizationProvider.Optimizely.Sandbox.Business.UIDescriptors;

/// <summary>
/// Describes how the UI should appear for <see cref="ContainerPage"/> content.
/// </summary>
[UIDescriptorRegistration]
public class ContainerPageUIDescriptor : UIDescriptor<ContainerPage>
{
    public ContainerPageUIDescriptor()
        : base(ContentTypeCssClassNames.Container)
    {
        DefaultView = CmsViewNames.AllPropertiesView;
    }
}
