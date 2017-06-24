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
            var visited = new List<Tuple<string, long>>();

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
                    var existing = visited.FirstOrDefault(v => v.Item1 == path);

                    // try to find parent
                    var existingParent = visited.FirstOrDefault(v => v.Item1 == parentPath);

                    if(existing == null)
                    {
                        result.Add(new ResourceTreeItem(_id,
                                                        existingParent?.Item2,
                                                        defragmented[ix],
                                                        resource.Key,
                                                        defragmented.Length == ix + 1,
                                                        defragmented.Length == ix + 1 ? resource.Value : null,
                                                        resource.AllowDelete,
                                                        resource.IsHidden));

                        visited.Add(new Tuple<string, long>(path, _id));

                        _id++;
                    }
                }
            }

            return result;
        }
    }
}
