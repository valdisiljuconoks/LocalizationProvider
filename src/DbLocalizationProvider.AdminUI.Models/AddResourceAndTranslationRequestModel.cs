// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Add a resource translation
/// </summary>
[JsonObject]
public class AddResourceAndTranslationRequestModel
{
    /// <summary>
    /// Resource key
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// For which language
    /// </summary>
    [JsonProperty("language")]
    public string Language { get; set; }

    /// <summary>
    /// What is new translation
    /// </summary>
    [JsonProperty("translation")]
    public string Translation { get; set; }
}
