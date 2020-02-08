using System;
using System.Runtime.Serialization;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Thrown when there is recursive resource reference resulting in stack overflow.
    /// </summary>
    public class RecursiveResourceReferenceException : Exception
    {
        public RecursiveResourceReferenceException() { }

        public RecursiveResourceReferenceException(string message) : base(message) { }

        public RecursiveResourceReferenceException(string message, Exception innerException) : base(message, innerException) { }

        protected RecursiveResourceReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
