using DbLocalizationProvider.Optimizely.Sandbox.Business;
using DbLocalizationProvider.Optimizely.Sandbox.Models.Pages;
using DbLocalizationProvider.Optimizely.Sandbox.Models.ViewModels;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Framework.Web.Mvc;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.Optimizely.Sandbox.Controllers;

// Note: as the content area rendering on Alloy is customized we create ContentArea instances
// which we render in the preview view in order to provide editors with a preview which is as
// realistic as possible. In other contexts we could simply have passed the block to the
// view and rendered it using Html.RenderContentData
[TemplateDescriptor(
    Inherited = true,
    TemplateTypeCategory = TemplateTypeCategories.MvcController, //Required as controllers for blocks are registered as MvcPartialController by default
    Tags = [RenderingTags.Preview, RenderingTags.Edit],
    AvailableWithoutTag = false)]
[VisitorGroupImpersonation]
[RequireClientResources]
public class PreviewController(
    IContentLoader contentLoader,
    TemplateResolver templateResolver,
    DisplayOptions displayOptions) : ActionControllerBase, IRenderTemplate<BlockData>, IModifyLayout
{
    public IActionResult Index(IContent currentContent)
    {
        // As the layout requires a page for title etc we "borrow" the start page
        var startPage = contentLoader.Get<StartPage>(ContentReference.StartPage);

        var model = new PreviewModel(startPage, currentContent);

        var supportedDisplayOptions = displayOptions
            .Select(x => new { x.Tag, x.Name, Supported = SupportsTag(currentContent, x.Tag) })
            .ToList();

        if (supportedDisplayOptions.Any(x => x.Supported))
        {
            foreach (var displayOption in supportedDisplayOptions)
            {
                var contentArea = new ContentArea();

                contentArea.Items.Add(new ContentAreaItem
                {
                    ContentLink = currentContent.ContentLink
                });

                var areaModel = new PreviewModel.PreviewArea
                {
                    Supported = displayOption.Supported,
                    AreaTag = displayOption.Tag,
                    AreaName = displayOption.Name,
                    ContentArea = contentArea
                };

                model.Areas.Add(areaModel);
            }
        }

        return View(model);
    }

    private bool SupportsTag(IContent content, string tag)
    {
        var templateModel = templateResolver.Resolve(
            HttpContext,
            content.GetOriginalType(),
            content,
            TemplateTypeCategories.MvcPartial,
            tag);

        return templateModel != null;
    }

    public void ModifyLayout(LayoutModel layoutModel)
    {
        layoutModel.HideHeader = true;
        layoutModel.HideFooter = true;
    }
}
