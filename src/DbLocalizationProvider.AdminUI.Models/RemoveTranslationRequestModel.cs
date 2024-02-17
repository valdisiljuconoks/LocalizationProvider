// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Model to pass to the server if you want to remove the translation for given resource in given language.
/// </summary>
[JsonObject]
public class RemoveTranslationRequestModel
{
    /// <summary>
    /// Resource key
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// Language of the translation
    /// </summary>
    [JsonProperty("language")]
    public string Language { get; set; }
}
