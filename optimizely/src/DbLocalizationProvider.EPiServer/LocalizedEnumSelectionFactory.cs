// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.EPiServer;

/// <inheritdoc />
public class LocalizedEnumSelectionFactory<TEnum> : ISelectionFactory where TEnum : struct
{
    /// <inheritdoc />
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var values = Enum.GetValues(typeof(TEnum))
                         .Cast<Enum>();

        var provider = ServiceLocator.Current.GetRequiredService<ILocalizationProvider>();

        foreach(var value in values)
        {
            yield return new SelectItem
            {
                Text = provider.Translate(value),
                Value = value
            };
        }
    }
}
