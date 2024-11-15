// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.EPiServer.Categories;

[AttributeUsage(AttributeTargets.Class)]
public class LocalizedCategoryAttribute : LocalizedResourceAttribute { }
