// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Export
{
    /// <summary>
    /// Result of the export operation.
    /// </summary>
    public class ExportResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportResult" /> class.
        /// </summary>
        /// <param name="serializedData">The serialized data.</param>
        /// <param name="fileMimeType">Type of the file MIME.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="ArgumentNullException">
        /// serializedData
        /// or
        /// fileMimeType
        /// or
        /// fileName
        /// </exception>
        public ExportResult(string serializedData, string fileMimeType, string fileName)
        {
            SerializedData = serializedData ?? throw new ArgumentNullException(nameof(serializedData));
            FileMimeType = fileMimeType ?? throw new ArgumentNullException(nameof(fileMimeType));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        /// <summary>
        /// Gets or sets exported data as serialized content.
        /// </summary>
        public string SerializedData { get; set; }

        /// <summary>
        /// Gets or sets the type of the file MIME.
        /// </summary>
        public string FileMimeType { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; }
    }
}
