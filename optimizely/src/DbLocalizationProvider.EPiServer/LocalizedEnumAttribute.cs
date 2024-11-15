// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;

namespace DbLocalizationProvider.EPiServer;

public class LocalizedEnumAttribute : Attribute, IMetadataExtender
{
    public LocalizedEnumAttribute(Type enumType, bool isManySelection = false)
    {
        EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
        IsManySelection = isManySelection;
    }

    public Type EnumType { get; set; }
    public bool IsManySelection { get; }

    /// <inheritdoc />
    public void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        metadata.ClientEditingClass = "epi-cms/contentediting/editors/" + (IsManySelection ? "CheckBoxListEditor" : "SelectionEditor");
        metadata.SelectionFactoryType = typeof(LocalizedEnumSelectionFactory<>).MakeGenericType(EnumType);
    }
}
