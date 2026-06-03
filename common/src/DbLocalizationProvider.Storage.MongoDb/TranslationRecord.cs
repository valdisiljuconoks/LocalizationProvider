// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Storage.MongoDb;

public class TranslationRecord
{
    public required int Id { get; set; }
    public required string Value { get; set; }
    public required string Language { get; set; }
    public required DateTime ModificationDate { get; set; }
}
