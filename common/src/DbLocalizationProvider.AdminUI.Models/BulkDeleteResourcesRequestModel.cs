// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Payload for bulk-deleting multiple resources.
/// </summary>
[JsonObject]
public class BulkDeleteResourcesRequestModel
{
    /// <summary>
    /// Resource keys to delete.
    /// </summary>
    [JsonProperty("keys")]
    public string[] Keys { get; set; } = [];
}
