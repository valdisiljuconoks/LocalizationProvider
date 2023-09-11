// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// When you  want to remove resource, pass this model to the API
/// </summary>
[JsonObject]
public class DeleteResourceRequestModel
{
    /// <summary>
    /// Resource key
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }
}
