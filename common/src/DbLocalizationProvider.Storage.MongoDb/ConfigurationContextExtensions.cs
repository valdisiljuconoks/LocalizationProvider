// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.Storage.MongoDb;

/// <summary>
/// Extension method to provide nice way to configure SQL Server as resource storage.
/// </summary>
public static class ConfigurationContextExtensions
{
    /// <summary>
    /// Well now it depends, if you cannot afford PostgreSql - this method is for you.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="connectionString">
    /// We need to know connection to your MongoDb server. It's not the name of the connectionString, but
    /// actual connectionString.
    /// </param>
    /// <param name="databaseName">Name of the database to use.</param>
    /// <returns></returns>
    public static ConfigurationContext UseMongo(
        this ConfigurationContext context,
        string connectionString,
        string databaseName)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        ArgumentException.ThrowIfNullOrEmpty(databaseName);

        Settings.ConnectionString = connectionString;
        Settings.DatabaseName = databaseName;

        context.TypeFactory.AddTransient<IResourceRepository, ResourceRepository>();
        context.TypeFactory.ForQuery<UpdateSchema.Command>().SetHandler<SchemaUpdater>();

        context.Services.AddSingleton<CollectionProvider>();
        context.Services.AddSingleton<CounterRepository>();
        
        return context;
    }
}
