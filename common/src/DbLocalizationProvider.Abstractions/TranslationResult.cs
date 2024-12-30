namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Result of the translation operation from AI service.
/// </summary>
/// <param name="IsSuccessful">Is operation successful?</param>
/// <param name="Result">Translated text to target language.</param>
/// <param name="Error">Filled in with problem details if any.</param>
public record TranslationResult(bool IsSuccessful, string? Result, Problem? Error = null)
{
    /// <summary>
    /// Translation was successful.
    /// </summary>
    /// <param name="text">Translated text to target language.</param>
    /// <returns>Successful translation result.</returns>
    public static TranslationResult Ok(string text)
    {
        return new TranslationResult(true, text);
    }

    /// <summary>
    /// Translation was failing.
    /// </summary>
    /// <param name="message">Error details if translation failed.</param>
    /// <param name="details">More details about an error.</param>
    /// <returns>Failed translation result.</returns>
    public static TranslationResult Failed(string message, string? details = null)
    {
        return new TranslationResult(false, null, new Problem(message, details));
    }
}

/// <summary>
/// More details about failed translation.
/// </summary>
/// <param name="Message">Message about an error.</param>
/// <param name="Details">More information about an error.</param>
public record Problem(string? Message, string? Details)
{
}
