// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Use this attribute to tell synchronize not to register decorated property as resource.
/// Useful in cases when you need to decorate class that has a lot of discoverable resources, but only few of them need
/// to be registered.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IgnoreAttribute : Attribute { }
