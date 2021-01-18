// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Sync;
using Microsoft.Azure.Cosmos.Table;

namespace DbLocalizationProvider.Storage.AzureTables
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
        public void Execute(UpdateSchema.Command command)
        {
            if (string.IsNullOrEmpty(Settings.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Storage connectionString is not initialized. Call ConfigurationContext.UseAzureTables() method.");
            }

            var storageAccount = CloudStorageAccount.Parse(Settings.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();

            var table = client.GetTableReference("LocalizationResources");
            table.CreateIfNotExists();
        }
    }
}
