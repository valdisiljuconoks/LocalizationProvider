// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// One of the main attributes of the library. Indicates that decorated class is kinda localized model.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class LocalizedModelAttribute : Attribute
{
    /// <summary>
    /// Used in cases when you need to override resource key generation and provide your own key.
    /// </summary>
    public string KeyPrefix { get; set; }

    /// <summary>
    /// Flag to indicate whether you care about your parents. If set to <c>false</c> - properties from parent type will not
    /// be considered as part of this type.
    /// </summary>
    public bool Inherited { get; set; } = true;

    /// <summary>
    /// Tells synchronized to take only properties (or fields) decorated *only* with <see cref="IncludeAttribute" />.
    /// </summary>
    public bool OnlyIncluded { get; set; } = false;
}
