// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Abstractions.Refactoring
{
    /// <summary>
    /// Use this attribute if you like to rename and refactor code a lot to support existing resource renames automagically.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class RenamedResourceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenamedResourceAttribute" /> class.
        /// </summary>
        public RenamedResourceAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenamedResourceAttribute" /> class.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="oldNamespace">The old namespace.</param>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace. - oldName</exception>
        public RenamedResourceAttribute(string oldName, string oldNamespace)
        {
            if (string.IsNullOrWhiteSpace(oldName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(oldName));
            }

            OldName = oldName;
            OldNamespace = oldNamespace;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenamedResourceAttribute" /> class.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        public RenamedResourceAttribute(string oldName) : this(oldName, null) { }

        /// <summary>
        /// Gets or sets the old name of the resource.
        /// </summary>
        public string OldName { get; set; }

        /// <summary>
        /// Gets or sets old namespace for the resource.
        /// </summary>
        public string OldNamespace { get; set; }
    }
}
