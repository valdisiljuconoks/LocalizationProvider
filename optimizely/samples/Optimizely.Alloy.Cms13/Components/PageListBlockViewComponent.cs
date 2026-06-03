using Optimizely.Alloy.Cms13.Business;
using Optimizely.Alloy.Cms13.Models.Blocks;
using Optimizely.Alloy.Cms13.Models.ViewModels;
using EPiServer.Filters;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Optimizely.Alloy.Cms13.Components;

public class PageListBlockViewComponent(
    ContentLocator contentLocator,
    IContentLoader contentLoader) : BlockComponent<PageListBlock>
{
    protected override IViewComponentResult InvokeComponent(PageListBlock currentContent)
    {
        var pages = FindPages(currentContent);

        pages = Sort(pages, currentContent.SortOrder);

        if (currentContent.Count > 0)
        {
            pages = pages.Take(currentContent.Count);
        }

        var model = new PageListModel(currentContent)
        {
            Pages = pages.Cast<PageData>()
        };

        ViewData.GetEditHints<PageListModel, PageListBlock>()
            .AddConnection(x => x.Heading, x => x.Heading);

        return View(model);
    }

    private IEnumerable<PageData> FindPages(PageListBlock currentBlock)
    {
        IEnumerable<PageData> pages;
        var listRoot = currentBlock.Root;

        if (ContentReference.IsNullOrEmpty(listRoot))
        {
            return Enumerable.Empty<PageData>();
        }

        if (currentBlock.Recursive)
        {
            if (currentBlock.PageTypeFilter is not null)
            {
                pages = contentLocator.FindPagesByPageType(listRoot, true, currentBlock.PageTypeFilter.ID);
            }
            else
            {
                pages = contentLocator.GetAll<PageData>(listRoot);
            }
        }
        else
        {
            if (currentBlock.PageTypeFilter is not null)
            {
                pages = contentLoader
                    .GetChildren<PageData>(listRoot)
                    .Where(p => p.ContentTypeID == currentBlock.PageTypeFilter.ID);
            }
            else
            {
                pages = contentLoader.GetChildren<PageData>(listRoot);
            }
        }

        if (currentBlock.CategoryFilter is not null && !currentBlock.CategoryFilter.IsEmpty)
        {
            pages = pages.Where(x => x.Category.Intersect(currentBlock.CategoryFilter).Any());
        }

        return pages;
    }

    private static IEnumerable<PageData> Sort(IEnumerable<PageData> pages, FilterSortOrder sortOrder)
    {
        var sortFilter = new FilterSort(sortOrder);
        sortFilter.Sort([.. pages.ToList()]);
        return pages;
    }
}
