using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.AdminUI
{
    public class ResourceTreeSorter
    {
        public ICollection<ResourceTreeItem> Sort(ICollection<ResourceTreeItem> list)
        {
            var result = new List<ResourceTreeItem>();

            foreach (var rootResource in list.Where(r => r.ParentId == null).OrderBy(r => r.ResourceKey))
            {
                result.Add(rootResource);
                SortRecursive(rootResource, list, ref result);
            }

            return result;
        }

        private void SortRecursive(ResourceTreeItem parentResource, ICollection<ResourceTreeItem> list, ref List<ResourceTreeItem> result)
        {
            var childNodes = list.Where(r => r.ParentId == parentResource.Id).OrderBy(r => r.ResourceKey);
            foreach (var childNode in childNodes)
            {
                result.Add(childNode);
                SortRecursive(childNode, list, ref result);
            }
        }
    }
}
