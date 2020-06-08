// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Npgsql;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    public static class NpgsqlParameterCollectionExtensions
    {
        public static NpgsqlParameter AddSafeWithValue(
            this NpgsqlParameterCollection collection,
            string parameterName,
            object value) =>
            collection.AddWithValue(parameterName, value ?? DBNull.Value);
    }
}
