// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class UpdateResourceNotesHandlerTests
{
    private readonly FakeRepository _repository = new();
    private readonly List<string> _cacheRemoved = [];
    private readonly UpdateResourceNotes.Handler _handler;

    public UpdateResourceNotesHandlerTests()
    {
        var ctx = new ConfigurationContext();
        ctx.CacheManager.OnRemove += e => _cacheRemoved.Add(e.CacheKey);
        _handler = new UpdateResourceNotes.Handler(Options.Create(ctx), _repository);
    }

    [Fact]
    public void SetsNotes_EvictsCache_WithoutTouchingIsModified()
    {
        _repository.Seed(new LocalizationResource("a", false)
        {
            IsModified = false, Author = "test", ModificationDate = DateTime.UtcNow
        });

        _handler.Execute(new UpdateResourceNotes.Command("a", "documents placeholder {0}"));

        var updated = _repository.GetByKey("a");
        Assert.Equal("documents placeholder {0}", updated!.Notes);
        // editing a note must NOT freeze code-sync of translations
        Assert.False(updated.IsModified);
        Assert.Equal(1, _repository.UpdateResourceCallCount);
        Assert.Contains(CacheKeyHelper.BuildKey("a"), _cacheRemoved);
    }

    [Fact]
    public void ClearsNotes_WhenNull()
    {
        _repository.Seed(new LocalizationResource("a", false) { Notes = "old", ModificationDate = DateTime.UtcNow });

        _handler.Execute(new UpdateResourceNotes.Command("a", null));

        Assert.Null(_repository.GetByKey("a")!.Notes);
    }

    [Fact]
    public void UnknownKey_NoOp()
    {
        _handler.Execute(new UpdateResourceNotes.Command("missing", "x"));

        Assert.Equal(0, _repository.UpdateResourceCallCount);
        Assert.Empty(_cacheRemoved);
    }

    [Fact]
    public void NullCommand_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _handler.Execute(null!));
    }

    private sealed class FakeRepository : IResourceRepository
    {
        private readonly Dictionary<string, LocalizationResource> _resources = new();

        public int UpdateResourceCallCount { get; private set; }

        public void Seed(params LocalizationResource[] resources)
        {
            foreach (var r in resources)
            {
                _resources[r.ResourceKey] = r;
            }
        }

        public LocalizationResource? GetByKey(string resourceKey) =>
            _resources.TryGetValue(resourceKey, out var r) ? r : null;

        public void UpdateResource(LocalizationResource resource)
        {
            UpdateResourceCallCount++;
            _resources[resource.ResourceKey] = resource;
        }

        public IEnumerable<LocalizationResource> GetAll() => _resources.Values;
        public IEnumerable<CultureInfo> GetAvailableLanguages(bool includeInvariant) => [];
        public void InsertResource(LocalizationResource resource) => throw new NotImplementedException();
        public void AddTranslation(LocalizationResource resource, LocalizationResourceTranslation translation) => throw new NotImplementedException();
        public void UpdateTranslation(LocalizationResource resource, LocalizationResourceTranslation translation) => throw new NotImplementedException();
        public void DeleteResource(LocalizationResource resource) => throw new NotImplementedException();
        public void DeleteResources(IEnumerable<LocalizationResource> resources) => throw new NotImplementedException();
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
