// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Use this attribute to set the initial notes (comment) for a resource. Notes are per-resource (not per-language)
/// and are shown and editable in the AdminUI. Use it to document non-obvious keys or the meaning of placeholders.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
public class NotesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotesAttribute" /> class.
    /// </summary>
    /// <param name="value">The notes (comment) for the resource.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
    public NotesAttribute(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the notes (comment) for the resource.
    /// </summary>
    public string Value { get; }
}
