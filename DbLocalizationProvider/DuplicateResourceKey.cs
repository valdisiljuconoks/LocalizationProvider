using System;
using System.Runtime.Serialization;

namespace DbLocalizationProvider
{
    public class DuplicateResourceKey : Exception
    {
        public DuplicateResourceKey() { }

        public DuplicateResourceKey(string message) : base(message) { }

        public DuplicateResourceKey(string message, Exception innerException) : base(message, innerException) { }

        protected DuplicateResourceKey(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
