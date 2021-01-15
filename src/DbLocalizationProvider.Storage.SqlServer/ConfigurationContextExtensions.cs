// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Storage.SqlServer
{
    /// <summary>
    /// Extension method to provide nice way to configure SQL Server as resource storage.
    /// </summary>
    public static class ConfigurationContextExtensions
    {
        /// <summary>
        /// If you can afford SQL Server - this method is for you.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="connectionString">
        /// We will need to know connectionString to your SQL Server. It's not the name of the connectionString, but
        /// actual connectionString.
        /// </param>
        /// <returns></returns>
        public static ConfigurationContext UseSqlServer(this ConfigurationContext context, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            Settings.DbContextConnectionString = connectionString;

            context.TypeFactory.AddTransient<IResourceRepository, ResourceRepository>();

            context.TypeFactory.ForQuery<UpdateSchema.Command>().SetHandler<SchemaUpdater>();
            context.TypeFactory.ForQuery<SyncResources.Query>().SetHandler<ResourceSynchronizer>();

            context.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResourcesHandler>();
            context.TypeFactory.ForQuery<GetResource.Query>().SetHandler<GetResourceHandler>();
            context.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslationHandler>();

            context.TypeFactory.ForCommand<RemoveTranslation.Command>().SetHandler<RemoveTranslationHandler>();

            return context;
        }
    }
}
