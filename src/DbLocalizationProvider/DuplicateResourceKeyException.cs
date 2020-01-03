// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Runtime.Serialization;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     If synchronizer will detect duplicate keys after scanner whole codebase - this exception will be thrown.
    /// </summary>
    public class DuplicateResourceKeyException : Exception
    {
        public DuplicateResourceKeyException() { }

        public DuplicateResourceKeyException(string message) : base(message) { }

        public DuplicateResourceKeyException(string message, Exception innerException) : base(message, innerException) { }

        protected DuplicateResourceKeyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
