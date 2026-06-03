using System.Globalization;
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
    // Azure Translator accepts up to 1000 array elements per request; stay well under that.
    private const int BatchSize = 100;

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
    public async Task<TranslationResult> TranslateAsync(string inputText, CultureInfo targetLanguage, CultureInfo sourceLanguage)
    {
        var results = await TranslateAsync([inputText], targetLanguage, sourceLanguage).ConfigureAwait(false);
        return results[0];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TranslationResult>> TranslateAsync(
        IReadOnlyList<string> inputTexts,
        CultureInfo targetLanguage,
        CultureInfo sourceLanguage)
    {
        if (inputTexts.Count == 0)
        {
            return [];
        }

        var credential = new AzureKeyCredential(_options.AccessKey);
        var client = new TextTranslationClient(credential, _options.Region);

        // Invariant culture has an empty name - let Azure auto-detect the source language in that case.
        var sourceCode = string.IsNullOrEmpty(sourceLanguage?.Name) ? null : sourceLanguage.Name;
        var results = new List<TranslationResult>(inputTexts.Count);

        foreach (var chunk in inputTexts.Chunk(BatchSize))
        {
            try
            {
                var response = await client.TranslateAsync(targetLanguage.Name, chunk, sourceCode).ConfigureAwait(false);
                var items = response.Value;

                for (var i = 0; i < chunk.Length; i++)
                {
                    var translations = items[i].Translations;
                    results.Add(translations is { Count: > 0 }
                        ? TranslationResult.Ok(translations[0].Text)
                        : TranslationResult.Failed("Failed to translate. Translations from Azure Cognitive Service are empty."));
                }
            }
            catch (RequestFailedException exception)
            {
                _logger.Error($"Failed to auto-translate to `{targetLanguage}`.", exception);

                for (var i = 0; i < chunk.Length; i++)
                {
                    results.Add(TranslationResult.Failed($"Failed to translate. {exception.Message}"));
                }
            }
        }

        return results;
    }
}
