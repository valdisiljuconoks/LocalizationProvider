// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Payload for previewing batch translations. Previewing does not persist any changes.
/// </summary>
[JsonObject]
public class BatchTranslatePreviewRequestModel
{
    /// <summary>
    /// Resource keys to translate.
    /// </summary>
    [JsonProperty("keys")]
    public string[] Keys { get; set; } = [];

    /// <summary>
    /// Source language code to translate from. An empty value or <c>"invariant"</c> means the invariant (master) text.
    /// </summary>
    [JsonProperty("sourceLanguage")]
    public string? SourceLanguage { get; set; }

    /// <summary>
    /// Target language code to translate to.
    /// </summary>
    [JsonProperty("targetLanguage")]
    public string TargetLanguage { get; set; } = null!;

    /// <summary>
    /// When <c>true</c>, only resources without an existing target translation are translated.
    /// </summary>
    [JsonProperty("onlyEmpty")]
    public bool OnlyEmpty { get; set; }
}
