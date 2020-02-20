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
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateResourceTranslationsException"/> class.
        /// </summary>
        public DuplicateResourceTranslationsException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateResourceTranslationsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DuplicateResourceTranslationsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateResourceTranslationsException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public DuplicateResourceTranslationsException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateResourceTranslationsException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected DuplicateResourceTranslationsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
