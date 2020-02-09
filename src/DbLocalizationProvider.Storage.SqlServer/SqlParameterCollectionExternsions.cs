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
