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
