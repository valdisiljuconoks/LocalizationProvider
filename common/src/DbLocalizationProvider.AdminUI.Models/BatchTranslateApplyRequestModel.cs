// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI.Models;

/// <summary>
/// Payload for committing previously previewed batch translations.
/// </summary>
[JsonObject]
public class BatchTranslateApplyRequestModel
{
    /// <summary>
    /// Target language code the translations belong to.
    /// </summary>
    [JsonProperty("targetLanguage")]
    public string TargetLanguage { get; set; } = null!;

    /// <summary>
    /// Translations to persist.
    /// </summary>
    [JsonProperty("items")]
    public BatchTranslateApplyItem[] Items { get; set; } = [];
}

/// <summary>
/// A single translation to persist for the target language.
/// </summary>
[JsonObject]
public class BatchTranslateApplyItem
{
    /// <summary>
    /// Resource key.
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; } = null!;

    /// <summary>
    /// Translation value to store for the target language.
    /// </summary>
    [JsonProperty("translation")]
    public string Translation { get; set; } = null!;
}
