using System;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Xunit;

namespace DbLocalizationProvider.Storage.PostgreSql.Tests;

public class ResourceRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        Settings.DbContextConnectionString = _postgreSqlContainer.GetConnectionString();
        new SchemaUpdater().Execute(null);
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void CanSaveNewResource()
    {
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var repo = new ResourceRepository(wrapper);
        var original = new LocalizationResource("testKey", false){
            IsHidden = false,
            FromCode = false,
            IsModified = true,
            Notes = "a test describtion",
            Author = "test",
            ModificationDate = DateTime.Now
        };
        repo.InsertResource(original);
        var fromDB = repo.GetByKey(original.ResourceKey);
        Assert.Equal(original.Notes, fromDB.Notes);
    }


}
