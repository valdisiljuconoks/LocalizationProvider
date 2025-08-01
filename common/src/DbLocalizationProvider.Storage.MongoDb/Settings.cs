// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Storage.MongoDb;

internal static class Settings
{
    public static string ConnectionString { get; set; } = "mongodb://localhost:27017/";
    public static string DatabaseName { get; set; } = "Localization";
}
