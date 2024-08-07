// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Sync;
using Npgsql;

namespace DbLocalizationProvider.Storage.PostgreSql;

public class SchemaUpdater : ICommandHandler<UpdateSchema.Command>
{
    public void Execute(UpdateSchema.Command command)
    {
        if (string.IsNullOrEmpty(Settings.DbContextConnectionString))
        {
            throw new InvalidOperationException(
                "Storage connectionString is not initialized. Call ConfigurationContext.UsePostgreSql() method.");
        }

        // check db schema and update if needed
        EnsureDatabaseSchema();
    }

    private void EnsureDatabaseSchema()
    {
        using var conn = new NpgsqlConnection(Settings.DbContextConnectionString);
        conn.Open();

        var cmd = new NpgsqlCommand(
            "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'public' AND TABLE_NAME = 'LocalizationResources'")
        {
            Connection = conn
        };

        var reader = cmd.ExecuteReader();
        var existsTables = reader.HasRows;
        reader.Close();

        if (existsTables)
        {
            return;
        }

        // there is no tables, let's create
        cmd.CommandText = @"CREATE TABLE public.""LocalizationResources""
                        (
                            ""Id"" bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
                            ""Author""  character varying(100) COLLATE pg_catalog.""default"" NOT NULL,
                            ""FromCode"" boolean NOT NULL,
                            ""IsHidden"" boolean NOT NULL,
                            ""IsModified"" boolean NOT NULL,
                            ""ModificationDate"" timestamp without time zone NOT NULL,
                            ""ResourceKey"" character varying(800) NOT NULL,
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

        cmd.CommandText =
            @"CREATE INDEX ""ix_FK_LocalizationResourceTranslations_LocalizationResources_ResourceId"" ON public.""LocalizationResourceTranslations""(""ResourceId"")";
        cmd.ExecuteNonQuery();

        cmd.CommandText =
            @"CREATE UNIQUE INDEX ""ix_UniqueResourceKey"" ON public.""LocalizationResources"" USING btree (""ResourceKey"" ASC NULLS LAST)";
        cmd.ExecuteNonQuery();

        cmd.CommandText =
            @"CREATE UNIQUE INDEX ""ix_UniqueTranslationForLanguage"" ON public.""LocalizationResourceTranslations"" USING btree (""Language"" ASC NULLS LAST, ""ResourceId"" ASC NULLS LAST)";
        cmd.ExecuteNonQuery();
    }
}
