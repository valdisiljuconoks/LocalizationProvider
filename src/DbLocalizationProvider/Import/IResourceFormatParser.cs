// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Import;

/// <summary>
/// Resource import format parser
/// </summary>
public interface IResourceFormatParser
{
    /// <summary>
    /// Gets the name of the format.
    /// </summary>
    string FormatName { get; }

    /// <summary>
    /// Gets the supported file extensions.
    /// </summary>
    string[] SupportedFileExtensions { get; }

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Parses the specified file content.
    /// </summary>
    /// <param name="fileContent">Content of the file.</param>
    /// <returns>Returns list of resources from the file</returns>
    ParseResult Parse(string fileContent);
}
