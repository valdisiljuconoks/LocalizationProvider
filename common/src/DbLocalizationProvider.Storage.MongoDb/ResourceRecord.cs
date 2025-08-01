// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.Storage.MongoDb;

public class ResourceRecord
{
    public required int Id { get; set; }
    public required string ResourceKey { get; set; }
    public required DateTime ModificationDate { get; set; }
    public required string Author { get; set; }
    public required bool FromCode { get; set; }
    public required bool? IsModified { get; set; }
    public required bool? IsHidden { get; set; }
    public required string Notes { get; set; }
    public required List<TranslationRecord> Translations { get; set; } = [];
}
