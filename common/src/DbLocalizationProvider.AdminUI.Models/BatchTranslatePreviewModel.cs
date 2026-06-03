// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Result of a batch-translation preview: proposed translations that are not yet persisted.
/// </summary>
[JsonObject]
public class BatchTranslatePreviewModel
{
    /// <summary>
    /// Target language code the texts were translated to.
    /// </summary>
    [JsonProperty("language")]
    public string Language { get; set; } = null!;

    /// <summary>
    /// Proposed translations, one per translatable resource.
    /// </summary>
    [JsonProperty("results")]
    public BatchTranslateItem[] Results { get; set; } = [];
}

/// <summary>
/// A single proposed translation produced during preview.
/// </summary>
[JsonObject]
public class BatchTranslateItem
{
    /// <summary>
    /// Resource key.
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; } = null!;

    /// <summary>
    /// Source text that was translated.
    /// </summary>
    [JsonProperty("sourceText")]
    public string SourceText { get; set; } = null!;

    /// <summary>
    /// Proposed translation, or <c>null</c> when translation failed.
    /// </summary>
    [JsonProperty("translation")]
    public string? Translation { get; set; }

    /// <summary>
    /// Whether the translation succeeded.
    /// </summary>
    [JsonProperty("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Error message when <see cref="Success" /> is <c>false</c>.
    /// </summary>
    [JsonProperty("error")]
    public string? Error { get; set; }
}
