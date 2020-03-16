// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.Data.SqlClient;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public static class SqlDataReaderExtensions
    {
        public static string GetStringSafe(this SqlDataReader reader, string columnName)
        {
            var colIndex = reader.GetOrdinal(columnName);

            return !reader.IsDBNull(colIndex) ? reader.GetString(reader.GetOrdinal(columnName)) : null;
        }

        public static bool GetBooleanSafe(this SqlDataReader reader, string columnName)
        {
            var colIndex = reader.GetOrdinal(columnName);

            return !reader.IsDBNull(colIndex) && reader.GetBoolean(reader.GetOrdinal(columnName));
        }
    }
}
