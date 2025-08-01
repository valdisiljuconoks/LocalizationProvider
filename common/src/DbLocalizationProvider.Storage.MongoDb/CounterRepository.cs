// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using MongoDB.Driver;

namespace DbLocalizationProvider.Storage.MongoDb;

public class CounterRepository(CollectionProvider collectionProvider)
{
    private const string CollectionName = "Counters";

    private readonly IMongoCollection<CounterRecord> _collection =
        collectionProvider.GetCollection<CounterRecord>(CollectionName);

    public int GetNextCounterValue(string name)
    {
        var filter = Builders<CounterRecord>.Filter.Eq(c => c.Name, name);
        var update = Builders<CounterRecord>.Update.Inc(c => c.Value, 1);
        var options = new FindOneAndUpdateOptions<CounterRecord>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        var counter = _collection.FindOneAndUpdate(filter, update, options);

        return counter.Value;
    }
}
