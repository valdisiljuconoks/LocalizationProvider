using System;
using System.Runtime.Serialization;

namespace DbLocalizationProvider
{
    public class DuplicateResourceTranslationsException : Exception
    {
        public DuplicateResourceTranslationsException() { }

        public DuplicateResourceTranslationsException(string message) : base(message) { }

        public DuplicateResourceTranslationsException(string message, Exception innerException) : base(message, innerException) { }

        protected DuplicateResourceTranslationsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
