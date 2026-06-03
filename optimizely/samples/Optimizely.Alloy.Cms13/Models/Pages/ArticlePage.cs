namespace Optimizely.Alloy.Cms13.Models.Pages;

/// <summary>
/// Used primarily for publishing news articles on the website
/// </summary>
[SiteContentType(
    GroupName = Globals.GroupNames.News,
    GUID = "AEECADF2-3E89-4117-ADEB-F8D43565D2F4")]
[SiteImageUrl(Globals.StaticGraphicsFolderPath + "page-type-thumbnail-article.png")]
public class ArticlePage : StandardPage
{
    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);

        VisibleInMenu = false;
    }
}
