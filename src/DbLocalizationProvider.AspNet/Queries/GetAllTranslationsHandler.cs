// Copyright © 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AspNet.Queries
{
    public class GetAllTranslationsHandler : IQueryHandler<GetAllTranslations.Query, IEnumerable<ResourceItem>>
    {
        public IEnumerable<ResourceItem> Execute(GetAllTranslations.Query query)
        {
            var q = new GetAllResources.Query();
            var allResources = q.Execute().Where(r =>
                                                     r.ResourceKey.StartsWith(query.Key) &&
                                                     r.Translations.Any(t => t.Language == query.Language.Name)).ToList();

            if(!allResources.Any())
            {
                return Enumerable.Empty<ResourceItem>();
            }

            return allResources.Select(r => new ResourceItem(r.ResourceKey,
                                                             r.Translations.First(t => t.Language == query.Language.Name).Value,
                                                             query.Language)).ToList();
        }
    }
}
