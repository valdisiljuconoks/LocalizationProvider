// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class BulkDeleteResourcesHandlerTests
{
    private readonly FakeRepository _repository = new();
    private readonly List<string> _cacheRemoved = [];
    private readonly BulkDeleteResources.Handler _handler;

    public BulkDeleteResourcesHandlerTests()
    {
        var ctx = new ConfigurationContext();
        ctx.CacheManager.OnRemove += e => _cacheRemoved.Add(e.CacheKey);
        _handler = new BulkDeleteResources.Handler(Options.Create(ctx), _repository);
    }

    [Fact]
    public void EmptyKeys_NoOp()
    {
        _repository.Seed(NewResource("a"));

        _handler.Execute(new BulkDeleteResources.Command([]));

        Assert.NotNull(_repository.GetByKey("a"));
        Assert.Empty(_cacheRemoved);
    }

    [Fact]
    public void NullKeys_NoOp()
    {
        _repository.Seed(NewResource("a"));

        _handler.Execute(new BulkDeleteResources.Command(null!));

        Assert.NotNull(_repository.GetByKey("a"));
    }

    [Fact]
    public void DeletesExistingKeys_SkipsUnknown()
    {
        _repository.Seed(NewResource("a"), NewResource("b"), NewResource("c"));

        _handler.Execute(new BulkDeleteResources.Command(["a", "missing", "c"]));

        Assert.Null(_repository.GetByKey("a"));
        Assert.NotNull(_repository.GetByKey("b"));
        Assert.Null(_repository.GetByKey("c"));
        Assert.Equal(2, _cacheRemoved.Count);
        Assert.Contains(CacheKeyHelper.BuildKey("a"), _cacheRemoved);
        Assert.Contains(CacheKeyHelper.BuildKey("c"), _cacheRemoved);
    }

    [Fact]
    public void FromCodeResources_SilentlySkipped()
    {
        _repository.Seed(
            NewResource("user1"),
            NewResource("synced", fromCode: true),
            NewResource("user2"));

        _handler.Execute(new BulkDeleteResources.Command(["user1", "synced", "user2"]));

        Assert.Null(_repository.GetByKey("user1"));
        Assert.NotNull(_repository.GetByKey("synced"));
        Assert.Null(_repository.GetByKey("user2"));
        Assert.Equal(2, _cacheRemoved.Count);
        Assert.DoesNotContain(CacheKeyHelper.BuildKey("synced"), _cacheRemoved);
    }

    [Fact]
    public void IgnoreFromCode_DeletesEverything()
    {
        _repository.Seed(
            NewResource("user1"),
            NewResource("synced", fromCode: true));

        _handler.Execute(new BulkDeleteResources.Command(["user1", "synced"]) { IgnoreFromCode = true });

        Assert.Null(_repository.GetByKey("user1"));
        Assert.Null(_repository.GetByKey("synced"));
        Assert.Equal(2, _cacheRemoved.Count);
    }

    [Fact]
    public void EmptyOrNullKeyEntries_Ignored()
    {
        _repository.Seed(NewResource("a"));

        _handler.Execute(new BulkDeleteResources.Command(["", null!, "a"]));

        Assert.Null(_repository.GetByKey("a"));
    }

    [Fact]
    public void OnlyFromCodeSelected_RepositoryNotCalled()
    {
        _repository.Seed(NewResource("synced", fromCode: true));

        _handler.Execute(new BulkDeleteResources.Command(["synced"]));

        Assert.Equal(0, _repository.DeleteResourcesCallCount);
        Assert.Empty(_cacheRemoved);
    }

    private static LocalizationResource NewResource(string key, bool fromCode = false)
    {
        return new LocalizationResource(key, false)
        {
            FromCode = fromCode,
            Author = "test",
            ModificationDate = DateTime.UtcNow
        };
    }

    private sealed class FakeRepository : IResourceRepository
    {
        private readonly Dictionary<string, LocalizationResource> _resources = new();

        public int DeleteResourcesCallCount { get; private set; }

        public void Seed(params LocalizationResource[] resources)
        {
            foreach (var r in resources)
            {
                _resources[r.ResourceKey] = r;
            }
        }

        public LocalizationResource? GetByKey(string resourceKey)
        {
            return _resources.TryGetValue(resourceKey, out var r) ? r : null;
        }

        public void DeleteResource(LocalizationResource resource)
        {
            _resources.Remove(resource.ResourceKey);
        }

        public void DeleteResources(IEnumerable<LocalizationResource> resources)
        {
            DeleteResourcesCallCount++;
            foreach (var r in resources)
            {
                _resources.Remove(r.ResourceKey);
            }
        }

        public IEnumerable<LocalizationResource> GetAll() => _resources.Values;

        public IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant) => [];
        public void InsertResource(LocalizationResource resource) => throw new NotImplementedException();
        public void AddTranslation(LocalizationResource resource, LocalizationResourceTranslation translation) => throw new NotImplementedException();
        public void UpdateTranslation(LocalizationResource resource, LocalizationResourceTranslation translation) => throw new NotImplementedException();
        public void UpdateResource(LocalizationResource resource) => throw new NotImplementedException();
        public void DeleteAllResources() => _resources.Clear();
        public void DeleteTranslation(LocalizationResource resource, LocalizationResourceTranslation translation) => throw new NotImplementedException();
        public void ResetSyncStatus() => throw new NotImplementedException();
        public void RegisterDiscoveredResources(
            ICollection<DiscoveredResource> discoveredResources,
            Dictionary<string, LocalizationResource>? allResources,
            bool flexibleRefactoringMode,
            SyncSource source) => throw new NotImplementedException();
    }

}
