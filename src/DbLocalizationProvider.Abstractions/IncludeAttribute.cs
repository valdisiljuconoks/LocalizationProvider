// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    ///     Opposite meaning compared to <see cref="IgnoreAttribute" />. Wanna know more - read doc of the attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IncludeAttribute : Attribute { }
}
