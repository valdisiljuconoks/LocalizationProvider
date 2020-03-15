// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.Data.SqlClient;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public static class SqlParameterCollectionExternsions
    {
        public static SqlParameter AddSafeWithValue(this SqlParameterCollection collection, string parameterName, object value) =>
            collection.AddWithValue(parameterName, value ?? DBNull.Value);
    }
}
