using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DbLocalizationProvider.Storage.MongoDb;

public class CounterRecord
{
    [BsonId]
    public required ObjectId Id { get; set; }

    public required string Name { get; set; }

    public required int Value { get; set; }
}
