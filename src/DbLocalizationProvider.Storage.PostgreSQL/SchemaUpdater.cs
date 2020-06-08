// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Sync;
using Npgsql;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    public class SchemaUpdater : ICommandHandler<UpdateSchema.Command>
    {
        public void Execute(UpdateSchema.Command command)
        {
            if (string.IsNullOrEmpty(Settings.DbContextConnectionString))
            {
                throw new InvalidOperationException("Storage connectionString is not initialized. Call ctx.UsePostgreSql() method.");
            }

            // check db schema and update if needed
            EnsureDatabaseSchema();
        }

        private void EnsureDatabaseSchema()
        {
            using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(
                    "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'public' AND TABLE_NAME = 'LocalizationResources'")
                {
                    Connection = conn
                };

                var reader = cmd.ExecuteReader();
                var doesNotExistsTables = !reader.HasRows;
                reader.Close();

                if (doesNotExistsTables)
                {
                    // there is no tables, let's create
                    cmd.CommandText = @"CREATE TABLE public.""LocalizationResources""
                        (
                            ""Id"" bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
                            ""Author""  character varying(100) COLLATE pg_catalog.""default"" NOT NULL,
                            ""FromCode"" boolean NOT NULL,
                            ""IsHidden"" boolean NOT NULL,
                            ""IsModified"" boolean NOT NULL,
                            ""ModificationDate"" timestamp without time zone NOT NULL,
                            ""ResourceKey"" character varying(1000) NOT NULL,
                            ""Notes"" character varying(3000) NULL,
                            CONSTRAINT ""PK_LocalizationResources"" PRIMARY KEY (""Id""))
                        ";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE public.""LocalizationResourceTranslations""
                        (
                            ""Id"" bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
                            ""Language"" character varying(10) NOT NULL,
                            ""ResourceId"" bigint NOT NULL,
                            ""Value"" character varying NULL,
                            ""ModificationDate"" timestamp without time zone NOT NULL,
                        CONSTRAINT ""PK_LocalizationResourceTranslations"" PRIMARY KEY (""Id""))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"ALTER TABLE public.""LocalizationResourceTranslations""
                            ADD CONSTRAINT ""FK_LocalizationResourceTranslations_LocalizationResources_ResourceId"" FOREIGN KEY (""ResourceId"")
                            REFERENCES public.""LocalizationResources"" (""Id"") MATCH SIMPLE
                            ON UPDATE NO ACTION
                            ON DELETE CASCADE
                            NOT VALID";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE INDEX ""ix_FK_LocalizationResourceTranslations_LocalizationResources_ResourceId"" ON public.""LocalizationResourceTranslations""(""ResourceId"")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE UNIQUE INDEX ""ix_UniqueTranslationForLanguage"" ON public.""LocalizationResourceTranslations"" USING btree (""Language"" ASC NULLS LAST, ""ResourceId"" ASC NULLS LAST)";
                    cmd.ExecuteNonQuery();
                }
                //else
                //{
                //    // there is something - so we need to check version and append missing stuff
                //    // NOTE: for now assumption is that we start from previous 5.x version

                //    // Below is list of additions on top of 5.x in chronological order.
                //    //  #1 addition - add LocalizationResources.Notes
                //    cmd.CommandText = "SELECT COL_LENGTH('dbo.LocalizationResources', 'Notes')";
                //    var result = cmd.ExecuteScalar();

                //    if (result == DBNull.Value)
                //    {
                //        cmd.CommandText = "ALTER TABLE dbo.LocalizationResources ADD Notes NVARCHAR(3000) NULL";
                //        cmd.ExecuteNonQuery();
                //    }

                //    // #2 change - LocalizationResources.Author NOT NULL
                //    if (IsColumnNullable("LocalizationResources", "Author", cmd))
                //    {
                //        ConvertColumnNotNullable("LocalizationResources", "Author", "[NVARCHAR](100)", "'migration'", cmd);
                //    }

                //    // #3 change - LocalizationResources.IsHidden NOT NULL
                //    if (IsColumnNullable("LocalizationResources", "IsHidden", cmd))
                //    {
                //        ConvertColumnNotNullable("LocalizationResources", "IsHidden", "bit", "0", cmd);
                //    }

                //    // #4 change - LocalizationResources.IsModified NOT NULL
                //    if (IsColumnNullable("LocalizationResources", "IsModified", cmd))
                //    {
                //        ConvertColumnNotNullable("LocalizationResources", "IsModified", "bit", "0", cmd);
                //    }

                //    // #5 change - LocalizationResourceTranslations.Language NOT NULL
                //    if (IsColumnNullable("LocalizationResourceTranslations", "Language", cmd))
                //    {
                //        ConvertColumnNotNullable("LocalizationResourceTranslations", "Language", "[NVARCHAR](10)", "''", cmd);
                //    }

                //    // #6 change - LocalizationResourceTranslations.ResourceId + Language = UNIQUE
                //    cmd.CommandText =
                //        "SELECT index_id FROM sys.indexes WHERE name='ix_UniqueTranslationForLanguage' AND object_id = OBJECT_ID('dbo.LocalizationResourceTranslations')";
                //    result = cmd.ExecuteScalar();

                //    if (result == null)
                //    {
                //        cmd.CommandText = "CREATE UNIQUE INDEX [ix_UniqueTranslationForLanguage] ON [dbo].[LocalizationResourceTranslations] ([Language], [ResourceId])";
                //        cmd.ExecuteNonQuery();
                //    }

                //    // #7 change - add LocalizationResourceTranslations.ModificationDate
                //    cmd.CommandText = "SELECT COL_LENGTH('dbo.LocalizationResourceTranslations', 'ModificationDate')";
                //    result = cmd.ExecuteScalar();

                //    if (result == DBNull.Value)
                //    {
                //        cmd.CommandText = "ALTER TABLE dbo.LocalizationResourceTranslations ADD ModificationDate [DATETIME2](7) NULL";
                //        cmd.ExecuteNonQuery();

                //        cmd.CommandText = "UPDATE t SET t.ModificationDate = r.ModificationDate FROM dbo.LocalizationResourceTranslations t INNER JOIN LocalizationResources r ON r.id = t.ResourceId";
                //        cmd.ExecuteNonQuery();

                //        cmd.CommandText = "ALTER TABLE dbo.LocalizationResourceTranslations ALTER COLUMN ModificationDate [DATETIME2](7) NOT NULL";
                //        cmd.ExecuteNonQuery();
                //    }
                //}
            }
        }

        //private void ConvertColumnNotNullable(string tableName, string columnName, string dataType, string defaultValue, NpgsqlCommand cmd)
        //{
        //    cmd.CommandText = $"UPDATE dbo.{tableName} SET [{columnName}] = {defaultValue} WHERE [{columnName}] IS NULL";
        //    cmd.ExecuteNonQuery();

        //    cmd.CommandText = $"ALTER TABLE dbo.[{tableName}] ALTER COLUMN [{columnName}] {dataType} NOT NULL";
        //    cmd.ExecuteNonQuery();
        //}

        //private bool IsColumnNullable(string tableName, string columnName, NpgsqlCommand cmd)
        //{
        //    cmd.CommandText = $"SELECT is_nullable FROM sys.columns c JOIN sys.tables t ON t.object_id = c.object_id WHERE t.name = '{tableName}' and c.name = '{columnName}'";
        //    var result = cmd.ExecuteScalar();

        //    return result != DBNull.Value && (bool)result;
        //}
    }
}
