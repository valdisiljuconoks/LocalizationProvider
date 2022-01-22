// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Refactoring;

namespace DbLocalizationProvider.Sync
{
    internal class LocalizedForeignResourceTypeScanner : IResourceTypeScanner
    {
        private readonly ResourceKeyBuilder _keyBuilder;
        private readonly OldResourceKeyBuilder _oldKeyBuilder;
        private readonly ScanState _state;
        private readonly ConfigurationContext _configurationContext;
        private readonly DiscoveredTranslationBuilder _translationBuilder;
        private IResourceTypeScanner _actualScanner;

        public LocalizedForeignResourceTypeScanner(
            ResourceKeyBuilder keyBuilder,
            OldResourceKeyBuilder oldKeyBuilder,
            ScanState state,
            ConfigurationContext configurationContext,
            DiscoveredTranslationBuilder translationBuilder)
        {
            _keyBuilder = keyBuilder;
            _oldKeyBuilder = oldKeyBuilder;
            _state = state;
            _configurationContext = configurationContext;
            _translationBuilder = translationBuilder;
        }

        public bool ShouldScan(Type target)
        {
            if (target.BaseType == typeof(Enum))
            {
                _actualScanner = new LocalizedEnumTypeScanner(_keyBuilder, _translationBuilder);
            }
            else
            {
                _actualScanner =
                    new LocalizedResourceTypeScanner(_keyBuilder,
                                                     _oldKeyBuilder,
                                                     _state,
                                                     _configurationContext,
                                                     _translationBuilder);
            }

            return true;
        }

        public string GetResourceKeyPrefix(Type target, string keyPrefix = null)
        {
            return _actualScanner.GetResourceKeyPrefix(target, keyPrefix);
        }

        public ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix)
        {
            return _actualScanner.GetClassLevelResources(target, resourceKeyPrefix);
        }

        public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
        {
            var discoveredResources = _actualScanner.GetResources(target, resourceKeyPrefix);

            // check whether we need to scan also complex properties
            var includeComplex = _configurationContext.ForeignResources.Get(target)?.IncludeComplexProperties ?? false;
            if (includeComplex)
            {
                discoveredResources.ForEach(r =>
                {
                    if (!r.IsSimpleType)
                    {
                        r.IncludedExplicitly = true;
                    }
                });
            }

            return discoveredResources;
        }
    }
}
