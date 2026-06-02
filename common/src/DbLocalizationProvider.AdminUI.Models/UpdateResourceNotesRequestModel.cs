// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Update notes (comment) for a resource
/// </summary>
[JsonObject]
public class UpdateResourceNotesRequestModel
{
    /// <summary>
    /// Resource key
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// Notes (comment) for the resource. <c>null</c> or empty clears the notes.
    /// </summary>
    [JsonProperty("notes")]
    public string? Notes { get; set; }
}
