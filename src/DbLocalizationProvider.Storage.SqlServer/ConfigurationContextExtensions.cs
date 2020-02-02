using System;
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
        /// <param name="connectionString">We will need to know connectionString to your SQL Server. It's not the name of the connectionString, but actual connectionString.</param>
        /// <returns></returns>
        public static ConfigurationContext UseSqlServer(this ConfigurationContext context, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            Settings.DbContextConnectionString = connectionString;

            ConfigurationContext.Current.TypeFactory.ForQuery<UpdateSchema.Command>().SetHandler<SchemaUpdater>();
            ConfigurationContext.Current.TypeFactory.ForQuery<SyncResources.Query>().SetHandler<ResourceSynchronizer>();

            ConfigurationContext.Current.TypeFactory.ForQuery<AvailableLanguages.Query>().SetHandler<AvailableLanguagesHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<GetAllResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetResource.Query>().SetHandler<GetResourceHandler>();
            ConfigurationContext.Current.TypeFactory.ForQuery<GetTranslation.Query>().SetHandler<GetTranslationHandler>();

            ConfigurationContext.Current.TypeFactory.ForCommand<CreateNewResources.Command>().SetHandler<CreateNewResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<DeleteAllResources.Command>().SetHandler<DeleteAllResourcesHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<DeleteResource.Command>().SetHandler<DeleteResourceHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<RemoveTranslation.Command>().SetHandler<RemoveTranslationHandler>();
            ConfigurationContext.Current.TypeFactory.ForCommand<CreateOrUpdateTranslation.Command>().SetHandler<CreateOrUpdateTranslationHandler>();

            return context;
        }
    }
}
