using System;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Xunit;

namespace DbLocalizationProvider.Storage.PostgreSql.Tests;

public class ResourceRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16.4")
        .Build();

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
        var original = new LocalizationResource("testKey", false)
        {
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

    [Fact]
    public void DeleteResources_RemovesSelectedAndKeepsRest()
    {
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var repo = new ResourceRepository(wrapper);

        repo.InsertResource(Build("bulk.a"));
        repo.InsertResource(Build("bulk.b"));
        repo.InsertResource(Build("bulk.c"));

        var a = repo.GetByKey("bulk.a");
        var c = repo.GetByKey("bulk.c");

        repo.AddTranslation(a, new LocalizationResourceTranslation { Language = "en", Value = "A-en", ResourceId = a.Id, ModificationDate = DateTime.UtcNow });
        repo.AddTranslation(c, new LocalizationResourceTranslation { Language = "en", Value = "C-en", ResourceId = c.Id, ModificationDate = DateTime.UtcNow });

        repo.DeleteResources([a, c]);

        Assert.Null(repo.GetByKey("bulk.a"));
        Assert.NotNull(repo.GetByKey("bulk.b"));
        Assert.Null(repo.GetByKey("bulk.c"));
    }

    [Fact]
    public void DeleteResources_EmptyCollection_NoOp()
    {
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var repo = new ResourceRepository(wrapper);

        repo.InsertResource(Build("keep.me"));

        repo.DeleteResources([]);

        Assert.NotNull(repo.GetByKey("keep.me"));
    }

    private static LocalizationResource Build(string key)
    {
        return new LocalizationResource(key, false)
        {
            Author = "test",
            FromCode = false,
            IsHidden = false,
            IsModified = true,
            ModificationDate = DateTime.UtcNow
        };
    }
}
