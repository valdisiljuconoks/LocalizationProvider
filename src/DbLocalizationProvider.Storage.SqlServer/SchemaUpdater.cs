// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Sync;
using Microsoft.Data.SqlClient;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public class SchemaUpdater : ICommandHandler<UpdateSchema.Command>
    {
        public void Execute(UpdateSchema.Command command)
        {
            if (string.IsNullOrEmpty(Settings.DbContextConnectionString))
            {
                throw new InvalidOperationException("Storage connectionString is not initialized. Call ctx.UseSqlServer() method.");
            }

            // check db schema and update if needed
            EnsureDatabaseSchema();
        }

        private void EnsureDatabaseSchema()
        {
            using (var conn = new SqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(
                    "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LocalizationResources'")
                {
                    Connection = conn
                };

                var reader = cmd.ExecuteReader();
                var doesNotExistsTables = !reader.HasRows;
                reader.Close();

                if (doesNotExistsTables)
                {
                    // there is no tables, let's create
                    cmd.CommandText = @"
                        CREATE TABLE [dbo].[LocalizationResources]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
                            [Author] [NVARCHAR](100) NOT NULL,
                            [FromCode] [BIT] NOT NULL,
                            [IsHidden] [BIT] NOT NULL,
                            [IsModified] [BIT] NOT NULL,
                            [ModificationDate] [DATETIME2](7) NOT NULL,
                            [ResourceKey] [NVARCHAR](1000) NOT NULL,
                            [Notes] [NVARCHAR](3000) NULL
                        CONSTRAINT [PK_LocalizationResources] PRIMARY KEY CLUSTERED ([Id] ASC))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        CREATE TABLE [dbo].[LocalizationResourceTranslations]
                        (
                            [Id] [INT] IDENTITY(1,1) NOT NULL,
                            [Language] [NVARCHAR](10) NOT NULL,
                            [ResourceId] [INT] NOT NULL,
                            [Value] [NVARCHAR](MAX) NULL,
                            [ModificationDate] [DATETIME2](7) NOT NULL,
                        CONSTRAINT [PK_LocalizationResourceTranslations] PRIMARY KEY CLUSTERED ([Id] ASC))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        ALTER TABLE [dbo].[LocalizationResourceTranslations]
                        WITH CHECK ADD CONSTRAINT [FK_LocalizationResourceTranslations_LocalizationResources_ResourceId]
                        FOREIGN KEY([ResourceId]) REFERENCES [dbo].[LocalizationResources] ([Id])
                        ON DELETE CASCADE";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE UNIQUE INDEX [ix_UniqueTranslationForLanguage] ON [dbo].[LocalizationResourceTranslations] ([Language], [ResourceId])";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // there is something - so we need to check version and append missing stuff
                    // NOTE: for now assumption is that we start from previous 5.x version

                    // Below is list of additions on top of 5.x in chronological order.
                    //  *** #1 addition - add LocalizationResources.Notes
                    cmd.CommandText = "SELECT COL_LENGTH('dbo.LocalizationResources', 'Notes')";
                    var result = cmd.ExecuteScalar();

                    if (result == DBNull.Value)
                    {
                        cmd.CommandText = "ALTER TABLE dbo.LocalizationResources ADD Notes NVARCHAR(3000) NULL";
                        cmd.ExecuteNonQuery();
                    }

                    // *** #2 change - LocalizationResources.Author NOT NULL
                    if (IsColumnNullable("LocalizationResources", "Author", cmd))
                    {
                        ConvertColumnNotNullable("LocalizationResources", "Author", "[NVARCHAR](100)", "'migration'", cmd);
                    }

                    // *** #3 change - LocalizationResources.IsHidden NOT NULL
                    if (IsColumnNullable("LocalizationResources", "IsHidden", cmd))
                    {
                        ConvertColumnNotNullable("LocalizationResources", "IsHidden", "bit", "0", cmd);
                    }

                    // *** #4 change - LocalizationResources.IsModified NOT NULL
                    if (IsColumnNullable("LocalizationResources", "IsModified", cmd))
                    {
                        ConvertColumnNotNullable("LocalizationResources", "IsModified", "bit", "0", cmd);
                    }

                    // *** #5 change - LocalizationResourceTranslations.Language NOT NULL
                    if (IsColumnNullable("LocalizationResourceTranslations", "Language", cmd))
                    {
                        ConvertColumnNotNullable("LocalizationResourceTranslations", "Language", "[NVARCHAR](10)", "''", cmd);
                    }

                    // *** #6 change - LocalizationResourceTranslations.ResourceId + Language = UNIQUE
                    cmd.CommandText =
                        "SELECT index_id FROM sys.indexes WHERE name='ix_UniqueTranslationForLanguage' AND object_id = OBJECT_ID('dbo.LocalizationResourceTranslations')";
                    result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        cmd.CommandText = "CREATE UNIQUE INDEX [ix_UniqueTranslationForLanguage] ON [dbo].[LocalizationResourceTranslations] ([Language], [ResourceId])";
                        cmd.ExecuteNonQuery();
                    }

                    // *** #7 change - add LocalizationResourceTranslations.ModificationDate
                    cmd.CommandText = "SELECT COL_LENGTH('dbo.LocalizationResourceTranslations', 'ModificationDate')";
                    result = cmd.ExecuteScalar();

                    if (result == DBNull.Value)
                    {
                        cmd.CommandText = "ALTER TABLE dbo.LocalizationResourceTranslations ADD ModificationDate [DATETIME2](7) NULL";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "UPDATE t SET t.ModificationDate = r.ModificationDate FROM dbo.LocalizationResourceTranslations t INNER JOIN LocalizationResources r ON r.Id = t.ResourceId";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "UPDATE dbo.LocalizationResourceTranslations SET ModificationDate = GETUTCDATE() WHERE ModificationDate IS NULL";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "ALTER TABLE dbo.LocalizationResourceTranslations ALTER COLUMN ModificationDate [DATETIME2](7) NOT NULL";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void ConvertColumnNotNullable(string tableName, string columnName, string dataType, string defaultValue, SqlCommand cmd)
        {
            cmd.CommandText = $"UPDATE dbo.{tableName} SET [{columnName}] = {defaultValue} WHERE [{columnName}] IS NULL";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"ALTER TABLE dbo.[{tableName}] ALTER COLUMN [{columnName}] {dataType} NOT NULL";
            cmd.ExecuteNonQuery();
        }

        private bool IsColumnNullable(string tableName, string columnName, SqlCommand cmd)
        {
            cmd.CommandText = $"SELECT is_nullable FROM sys.columns c JOIN sys.tables t ON t.object_id = c.object_id WHERE t.name = '{tableName}' and c.name = '{columnName}'";
            var result = cmd.ExecuteScalar();

            return result != DBNull.Value && (bool)result;
        }
    }
}
