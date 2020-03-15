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
                            [Author] [nvarchar](100) NULL,
                            [FromCode] [bit] NOT NULL,
                            [IsHidden] [bit] NULL,
                            [IsModified] [bit] NULL,
                            [ModificationDate] [datetime2](7) NOT NULL,
                            [ResourceKey] [nvarchar](1000) NOT NULL,
                            [Notes] [nvarchar](3000) NULL
                        CONSTRAINT [PK_LocalizationResources] PRIMARY KEY CLUSTERED ([Id] ASC))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        CREATE TABLE [dbo].[LocalizationResourceTranslations]
                        (
                            [Id] [INT] IDENTITY(1,1) NOT NULL,
                            [Language] [NVARCHAR](10) NULL,
                            [ResourceId] [INT] NOT NULL,
                            [Value] [NVARCHAR](MAX) NULL,
                        CONSTRAINT [PK_LocalizationResourceTranslations] PRIMARY KEY CLUSTERED ([Id] ASC))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        ALTER TABLE [dbo].[LocalizationResourceTranslations]
                        WITH CHECK ADD CONSTRAINT [FK_LocalizationResourceTranslations_LocalizationResources_ResourceId]
                        FOREIGN KEY([ResourceId]) REFERENCES [dbo].[LocalizationResources] ([Id])
                        ON DELETE CASCADE";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // there is something - so we need to check version and append missing stuff
                    // NOTE: for now assumption is that we start from previous 5.x version

                    // Below is list of additions on top of 5.x in chronological order.
                    //  #1 addition - notes column for resource
                    cmd.CommandText = "SELECT COL_LENGTH('dbo.LocalizationResources', 'Notes')";
                    var result = cmd.ExecuteScalar();

                    if (result == DBNull.Value)
                    {
                        cmd.CommandText = "ALTER TABLE dbo.LocalizationResources ADD Notes NVARCHAR(3000) NULL";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
