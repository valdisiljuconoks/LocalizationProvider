namespace DbLocalizationProvider.Translator.Azure;

/// <summary>
/// Options for Azure Cognitive Service translator implementation.
/// </summary>
public class CognitiveServicesOptions
{
    /// <summary>
    /// Access key for the service.
    /// </summary>
    public string AccessKey { get; set; } = null!;

    /// <summary>
    /// Region code where service is deployed.
    /// </summary>
    public string Region { get; set; } = null!;
}
