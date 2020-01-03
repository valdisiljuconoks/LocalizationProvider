// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Export
{
    public class ExportResult
    {
        public ExportResult(string serializedData, string fileMimeType, string fileName)
        {
            SerializedData = serializedData ?? throw new ArgumentNullException(nameof(serializedData));
            FileMimeType = fileMimeType ?? throw new ArgumentNullException(nameof(fileMimeType));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public string SerializedData { get; set; }

        public string FileMimeType { get; set; }

        public string FileName { get; set; }
    }
}
