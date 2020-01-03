// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider
{
    public class LocalizationResourceTranslation
    {
        public int Id { get; set; }

        public int ResourceId { get; set; }

        [JsonIgnore]
        public LocalizationResource LocalizationResource { get; set; }

        public string Language { get; set; }

        public string Value { get; set; }
    }
}
