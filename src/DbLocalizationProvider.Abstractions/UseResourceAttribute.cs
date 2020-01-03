// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    ///     Sometimes you just feel lazy enough to reuse already existing resources and not generating new ones. Well, this attribute does exactly that.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UseResourceAttribute : Attribute
    {
        /// <summary>
        ///     Creates new instance of this attribute. What else?
        /// </summary>
        /// <param name="targetContainer">References type that contains resource that will be used here.</param>
        /// <param name="propertyName">Name of the property to use for the reference</param>
        public UseResourceAttribute(Type targetContainer, string propertyName)
        {
            TargetContainer = targetContainer ?? throw new ArgumentNullException(nameof(targetContainer));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        /// <summary>
        ///     References type that contains resource that will be used here.
        /// </summary>
        public Type TargetContainer { get; }

        /// <summary>
        ///     Name of the property to use for the reference
        /// </summary>
        public string PropertyName { get; }
    }
}
