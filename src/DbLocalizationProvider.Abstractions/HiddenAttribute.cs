// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    ///     Use this attribute to hide resource in AdminUI
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
    public class HiddenAttribute : Attribute { }
}
