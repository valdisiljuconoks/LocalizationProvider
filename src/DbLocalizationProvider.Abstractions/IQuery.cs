// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Interface for queries. Localization provider internally is built using something similar to CQS pattern.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IQuery<out TResult> { }
