// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Runtime.Serialization;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     If synchronizer detects duplicate translations for the same resource key and the same culture - this exception will
    ///     be thrown.
    /// </summary>
    public class DuplicateResourceTranslationsException : Exception
    {
        public DuplicateResourceTranslationsException() { }

        public DuplicateResourceTranslationsException(string message) : base(message) { }

        public DuplicateResourceTranslationsException(string message, Exception innerException) : base(message, innerException) { }

        protected DuplicateResourceTranslationsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
