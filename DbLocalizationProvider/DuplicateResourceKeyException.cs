using System;
using System.Runtime.Serialization;

namespace DbLocalizationProvider
{
    public class DuplicateResourceKeyException : Exception
    {
        public DuplicateResourceKeyException() { }

        public DuplicateResourceKeyException(string message) : base(message) { }

        public DuplicateResourceKeyException(string message, Exception innerException) : base(message, innerException) { }

        protected DuplicateResourceKeyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
