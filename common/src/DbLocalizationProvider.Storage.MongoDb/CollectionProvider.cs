// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using MongoDB.Driver;

namespace DbLocalizationProvider.Storage.MongoDb;

public class CollectionProvider
{
    private readonly IMongoDatabase _mongoDatabase;

    public CollectionProvider()
    {
        var mongoClient = new MongoClient(Settings.ConnectionString);
        _mongoDatabase = mongoClient.GetDatabase(Settings.DatabaseName);
    }


    public IMongoCollection<TDocument> GetCollection<TDocument>(string name)
    {
        return _mongoDatabase.GetCollection<TDocument>(name);
    }
}
