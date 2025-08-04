using System;
using System.Threading;
using MongoDB.Driver;

namespace DbLocalizationProvider.Storage.MongoDb;

public class CounterRepository(CollectionProvider collectionProvider)
{
    private const string CollectionName = "Counters";
    private const int MaxRetries = 5;
    private static bool _initialized;
    private readonly IMongoCollection<CounterRecord> _collection = collectionProvider.GetCollection<CounterRecord>(CollectionName);

    // TODO: migrate to System.Threading.Lock when we will be on .NET 9
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        _semaphore.Wait();
        try
        {
            if (!_initialized)
            {
                _collection.Indexes.CreateOne(new CreateIndexModel<CounterRecord>(
                    Builders<CounterRecord>.IndexKeys.Ascending(c => c.Name),
                    new CreateIndexOptions { Unique = true }));
                _initialized = true;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public int GetNextCounterValue(string name)
    {
        EnsureInitialized();

        var filter = Builders<CounterRecord>.Filter.Eq(c => c.Name, name);
        var update = Builders<CounterRecord>.Update
            .Inc(c => c.Value, 1)
            .SetOnInsert(c => c.Name, name);

        var options = new FindOneAndUpdateOptions<CounterRecord>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                var counter = _collection.FindOneAndUpdate(filter, update, options);
                return counter.Value;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                // Another thread inserted the document concurrently.
                // Retry to safely increment the now-existing document.
            }
        }

        throw new InvalidOperationException(
                $"Failed to get next counter value for '{name}' after {MaxRetries} retries."
            );
    }
}
