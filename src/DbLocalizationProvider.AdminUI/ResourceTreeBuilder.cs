using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.AdminUI
{
    public class ResourceTreeBuilder
    {
        private long _id;

        public IEnumerable<ResourceTreeItem> BuildTree(List<ResourceListItem> resources)
        {
            var result = new List<ResourceTreeItem>();
            foreach (var resource in resources)
            {
                var defragmented = resource.Key.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                var path = string.Empty;
                var parentPath = string.Empty;

                // e.g.: MyNamespace.MyProject.AnotherResource
                for (var ix = 0; ix < defragmented.Length; ix++)
                {
                    path = !string.IsNullOrEmpty(path) ? string.Join(".", path, defragmented[ix]) : defragmented[ix];

                    if(ix > 0)
                        parentPath = !string.IsNullOrEmpty(parentPath)
                                         ? string.Join(".", parentPath, defragmented[ix - 1])
                                         : defragmented[ix - 1];

                    // try to find myself
                    var existing = result.FirstOrDefault(v => v.Path == path);

                    // try to find parent
                    var existingParent = result.FirstOrDefault(v => v.Path == parentPath);

                    if(existing == null)
                    {
                        result.Add(new ResourceTreeItem(_id,
                                                        existingParent?.Id,
                                                        defragmented[ix],
                                                        resource.Key,
                                                        defragmented.Length == ix + 1,
                                                        defragmented.Length == ix + 1 ? resource.Value : null,
                                                        resource.AllowDelete,
                                                        resource.IsHidden,
                                                        path));

                        _id++;
                    }
                }

                UpdateResourceVisibility(resource, defragmented, ref result);
            }

            return result;
        }

        private void UpdateResourceVisibility(ResourceListItem resource, string[] defragmented, ref List<ResourceTreeItem> result)
        {
            for (var ix = defragmented.Length; ix > 0; ix--)
            {
                var path = string.Join(".", defragmented.Take(ix));
                var item = result.FirstOrDefault(r => r.Path == path);

                if(item != null && !resource.IsHidden)
                    item.IsHidden = resource.IsHidden;
            }
        }
    }
}
