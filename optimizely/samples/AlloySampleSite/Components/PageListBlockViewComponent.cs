using AlloySampleSite.Business;
using AlloySampleSite.Models.Blocks;
using AlloySampleSite.Models.ViewModels;
using EPiServer;
using EPiServer.Cms.AspNetCore.Mvc;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AlloySampleSite.Controllers
{
    public class PageListBlockViewComponent : BlockComponent<PageListBlock>
    {
        private ContentLocator contentLocator;
        private IContentLoader contentLoader;
        public PageListBlockViewComponent(ContentLocator contentLocator, IContentLoader contentLoader)
        {
            this.contentLocator = contentLocator;
            this.contentLoader = contentLoader;
        }

        protected override IViewComponentResult InvokeComponent(PageListBlock currentBlock)
        {
            var pages = FindPages(currentBlock);

            pages = Sort(pages, currentBlock.SortOrder);

            if (currentBlock.Count > 0)
            {
                pages = pages.Take(currentBlock.Count);
            }

            var model = new PageListModel(currentBlock)
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
            if (currentBlock.Recursive)
            {
                if (currentBlock.PageTypeFilter != null)
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
                if (currentBlock.PageTypeFilter != null)
                {
                    pages = contentLoader.GetChildren<PageData>(listRoot)
                        .Where(p => p.ContentTypeID == currentBlock.PageTypeFilter.ID);
                }
                else
                {
                    pages = contentLoader.GetChildren<PageData>(listRoot);
                }
            }

            if (currentBlock.CategoryFilter != null && currentBlock.CategoryFilter.Any())
            {
                pages = pages.Where(x => x.Category.Intersect(currentBlock.CategoryFilter).Any());
            }
            return pages;
        }

        private IEnumerable<PageData> Sort(IEnumerable<PageData> pages, FilterSortOrder sortOrder)
        {
            var sortFilter = new FilterSort(sortOrder);
            sortFilter.Sort(new PageDataCollection(pages.ToList()));
            return pages;
        }
    }
}
