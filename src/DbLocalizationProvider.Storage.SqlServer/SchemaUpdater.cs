// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Sync;
using Microsoft.Data.SqlClient;

namespace DbLocalizationProvider.Storage.SqlServer
{
    /// <summary>
    /// Command to be executed when storage implementation is requested to get its affairs in order and initialize data structures if needed
    /// </summary>
    public class SchemaUpdater : ICommandHandler<UpdateSchema.Command>
    {
        /// <summary>
        /// Executes the command obviously.
        /// </summary>
        /// <param name="command"></param>
        public async Task Execute(UpdateSchema.Command command)
        {
            if (string.IsNullOrEmpty(Settings.DbContextConnectionString))
            {
                throw new InvalidOperationException(
                    "Storage connectionString is not initialized. Call ConfigurationContext.UseSqlServer() method.");
            }

            // check db schema and update if needed
            await EnsureDatabaseSchema();
        }

        private async Task EnsureDatabaseSchema()
        {
            await using var conn = new SqlConnection(Settings.DbContextConnectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LocalizationResources'")
            {
                Connection = conn
            };

            var reader = await cmd.ExecuteReaderAsync();
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
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = @"
                        CREATE TABLE [dbo].[LocalizationResourceTranslations]
                        (
                            [Id] [INT] IDENTITY(1,1) NOT NULL,
                            [Language] [NVARCHAR](10) NOT NULL,
                            [ResourceId] [INT] NOT NULL,
                            [Value] [NVARCHAR](MAX) NULL,
                            [ModificationDate] [DATETIME2](7) NOT NULL,
                        CONSTRAINT [PK_LocalizationResourceTranslations] PRIMARY KEY CLUSTERED ([Id] ASC))";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = @"
                        ALTER TABLE [dbo].[LocalizationResourceTranslations]
                        WITH CHECK ADD CONSTRAINT [FK_LocalizationResourceTranslations_LocalizationResources_ResourceId]
                        FOREIGN KEY([ResourceId]) REFERENCES [dbo].[LocalizationResources] ([Id])
                        ON DELETE CASCADE";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText =
                    "CREATE UNIQUE INDEX [ix_UniqueTranslationForLanguage] ON [dbo].[LocalizationResourceTranslations] ([Language], [ResourceId])";
                await cmd.ExecuteNonQueryAsync();
            }
            else
            {
                // there is something - so we need to check version and append missing stuff
                // NOTE: for now assumption is that we start from previous 5.x version

                // Below is list of additions on top of 5.x in chronological order.
                //  *** #1 addition - add LocalizationResources.Notes
                cmd.CommandText = "SELECT COL_LENGTH('dbo.LocalizationResources', 'Notes')";
                var result = await cmd.ExecuteScalarAsync();

                if (result == DBNull.Value)
                {
                    cmd.CommandText = "ALTER TABLE dbo.LocalizationResources ADD Notes NVARCHAR(3000) NULL";
                    await cmd.ExecuteNonQueryAsync();
                }

                // *** #2 change - LocalizationResources.Author NOT NULL
                if (await IsColumnNullable("LocalizationResources", "Author", cmd))
                {
                    await ConvertColumnNotNullable("LocalizationResources", "Author", "[NVARCHAR](100)", "'migration'", cmd);
                }

                // *** #3 change - LocalizationResources.IsHidden NOT NULL
                if (await IsColumnNullable("LocalizationResources", "IsHidden", cmd))
                {
                    await ConvertColumnNotNullable("LocalizationResources", "IsHidden", "bit", "0", cmd);
                }

                // *** #4 change - LocalizationResources.IsModified NOT NULL
                if (await IsColumnNullable("LocalizationResources", "IsModified", cmd))
                {
                    await ConvertColumnNotNullable("LocalizationResources", "IsModified", "bit", "0", cmd);
                }

                // *** #5 change - LocalizationResourceTranslations.Language NOT NULL
                if (await IsColumnNullable("LocalizationResourceTranslations", "Language", cmd))
                {
                    await ConvertColumnNotNullable("LocalizationResourceTranslations", "Language", "[NVARCHAR](10)", "''", cmd);
                }

                // *** #6 change - LocalizationResourceTranslations.ResourceId + Language = UNIQUE
                cmd.CommandText =
                    "SELECT index_id FROM sys.indexes WHERE name='ix_UniqueTranslationForLanguage' AND object_id = OBJECT_ID('dbo.LocalizationResourceTranslations')";
                result = await cmd.ExecuteScalarAsync();

                if (result == null)
                {
                    cmd.CommandText =
                        "CREATE UNIQUE INDEX [ix_UniqueTranslationForLanguage] ON [dbo].[LocalizationResourceTranslations] ([Language], [ResourceId])";
                    await cmd.ExecuteNonQueryAsync();
                }

                // *** #7 change - add LocalizationResourceTranslations.ModificationDate
                cmd.CommandText = "SELECT COL_LENGTH('dbo.LocalizationResourceTranslations', 'ModificationDate')";
                result = await cmd.ExecuteScalarAsync();

                if (result == DBNull.Value)
                {
                    cmd.CommandText =
                        "ALTER TABLE dbo.LocalizationResourceTranslations ADD ModificationDate [DATETIME2](7) NULL";
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText =
                        "UPDATE t SET t.ModificationDate = r.ModificationDate FROM dbo.LocalizationResourceTranslations t INNER JOIN LocalizationResources r ON r.Id = t.ResourceId";
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText =
                        "UPDATE dbo.LocalizationResourceTranslations SET ModificationDate = GETUTCDATE() WHERE ModificationDate IS NULL";
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText =
                        "ALTER TABLE dbo.LocalizationResourceTranslations ALTER COLUMN ModificationDate [DATETIME2](7) NOT NULL";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task ConvertColumnNotNullable(
            string tableName,
            string columnName,
            string dataType,
            string defaultValue,
            SqlCommand cmd)
        {
            cmd.CommandText = $"UPDATE dbo.{tableName} SET [{columnName}] = {defaultValue} WHERE [{columnName}] IS NULL";
            await cmd.ExecuteNonQueryAsync();

            cmd.CommandText = $"ALTER TABLE dbo.[{tableName}] ALTER COLUMN [{columnName}] {dataType} NOT NULL";
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<bool> IsColumnNullable(string tableName, string columnName, SqlCommand cmd)
        {
            cmd.CommandText =
                $"SELECT is_nullable FROM sys.columns c JOIN sys.tables t ON t.object_id = c.object_id WHERE t.name = '{tableName}' and c.name = '{columnName}'";
            var result = await cmd.ExecuteScalarAsync();

            return result != null && result != DBNull.Value && (bool)result;
        }
    }
}
