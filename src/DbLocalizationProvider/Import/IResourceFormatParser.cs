namespace DbLocalizationProvider.Import
{
    public interface IResourceFormatParser
    {
        string FormatName { get; }

        string[] SupportedFileExtensions { get; }

        string ProviderId { get; }

        ParseResult Parse(string fileContent);
    }
}
