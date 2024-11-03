using Azure;
using Azure.AI.Translation.Text;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Logging;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Translator.Azure;

/// <summary>
/// Translator implementation for Azure Cognitive Services.
/// </summary>
public class CognitiveServiceTranslator : ITranslatorService
{
    private readonly ILogger _logger;
    private readonly CognitiveServicesOptions _options;

    /// <summary>
    /// Creates new instance of translator service.
    /// </summary>
    /// <param name="options">We need some options class for the service to work properly.</param>
    /// <param name="logger">Area where to dump if something goes bananas.</param>
    public CognitiveServiceTranslator(IOptions<CognitiveServicesOptions> options, ILogger logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<string?> TranslateAsync(string inputText, string targetLanguage)
    {
        AzureKeyCredential credential = new(_options.AccessKey);
        TextTranslationClient client = new(credential, _options.Region);

        try
        {
            var response = await client.TranslateAsync(targetLanguage, inputText).ConfigureAwait(false);
            var translations = response.Value;
            var translation = translations.FirstOrDefault();

            return translation?.Translations?.FirstOrDefault()?.Text;
        }
        catch (RequestFailedException exception)
        {
            _logger.Error($"Failed to auto-translate to `{targetLanguage}`.", exception);
        }

        return default;
    }
}
