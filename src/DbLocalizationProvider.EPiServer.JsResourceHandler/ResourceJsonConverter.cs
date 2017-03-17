using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    public class ResourceJsonConverter
    {
        public JObject Convert(ICollection<LocalizationResource> resources, string langauge)
        {
            var result = new JObject();

            foreach (var resource in resources)
            {
                if(!resource.ResourceKey.Contains("."))
                    continue;

                var segments = resource.ResourceKey.Split(new[] { "." }, StringSplitOptions.None);
                var lastSegment = segments.Last();

                if(!resource.Translations.ExistsLanguage(langauge))
                    continue;

                var translation = resource.Translations.ByLanguage(langauge);

                segments.Aggregate(result,
                                   (e, segment) =>
                                   {
                                       if(e[segment] == null)
                                           e[segment] = new JObject();

                                       if(segment == lastSegment)
                                           e[segment] = translation;

                                       return e[segment] as JObject;
                                   });
            }

            return result;
        }
    }
}
