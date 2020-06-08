// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Npgsql;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    public static class NpgsqlDataReaderExtensions
    {
        public static string GetStringSafe(this NpgsqlDataReader reader, string columnName)
        {
            var colIndex = reader.GetOrdinal(columnName);

            return !reader.IsDBNull(colIndex) ? reader.GetString(reader.GetOrdinal(columnName)) : null;
        }

        public static bool GetBooleanSafe(this NpgsqlDataReader reader, string columnName)
        {
            var colIndex = reader.GetOrdinal(columnName);

            return !reader.IsDBNull(colIndex) && reader.GetBoolean(reader.GetOrdinal(columnName));
        }
    }
}
