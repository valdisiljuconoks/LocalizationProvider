using System;

namespace DbLocalizationProvider;

/// <summary>
/// Thrown when there is recursive resource reference resulting in stack overflow.
/// </summary>
public class RecursiveResourceReferenceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecursiveResourceReferenceException" /> class.
    /// </summary>
    public RecursiveResourceReferenceException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecursiveResourceReferenceException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RecursiveResourceReferenceException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecursiveResourceReferenceException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no
    /// inner exception is specified.
    /// </param>
    public RecursiveResourceReferenceException(string message, Exception innerException) : base(message, innerException) { }
}
