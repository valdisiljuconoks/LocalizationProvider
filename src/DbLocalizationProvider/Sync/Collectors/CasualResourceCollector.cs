// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Refactoring;

namespace DbLocalizationProvider.Sync.Collectors
{
    internal class CasualResourceCollector : IResourceCollector
    {
        private readonly OldResourceKeyBuilder _oldKeyKeyBuilder;
        private readonly DiscoveredTranslationBuilder _translationBuilder;

        public CasualResourceCollector(OldResourceKeyBuilder oldKeyKeyBuilder, DiscoveredTranslationBuilder translationBuilder)
        {
            _oldKeyKeyBuilder = oldKeyKeyBuilder;
            _translationBuilder = translationBuilder;
        }

        public IEnumerable<DiscoveredResource> GetDiscoveredResources(
            Type target,
            object instance,
            MemberInfo mi,
            string translation,
            string resourceKey,
            string resourceKeyPrefix,
            bool typeKeyPrefixSpecified,
            bool isHidden,
            string typeOldName,
            string typeOldNamespace,
            Type declaringType,
            Type returnType,
            bool isSimpleType)
        {
            // check if there are [ResourceKey] attributes
            var keyAttributes = mi.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            if (keyAttributes.Any())
            {
                yield break;
            }

            // check if there are [UseResource] attributes
            var useAttribute = mi.GetCustomAttribute<UseResourceAttribute>();
            if (useAttribute != null)
            {
                yield break;
            }

            var isResourceHidden = isHidden || mi.GetCustomAttribute<HiddenAttribute>() != null;
            var translations = _translationBuilder.GetAllTranslations(mi, resourceKey, translation);
            var oldResourceKeys =
                _oldKeyKeyBuilder.GenerateOldResourceKey(target,
                                                         mi.Name,
                                                         mi,
                                                         resourceKeyPrefix,
                                                         typeOldName,
                                                         typeOldNamespace);

            // if property is of type enumerable with simple generic argument type
            // we treat it as simple type and generate resource key for it
            if (IsPropertyACollectionOfScalar(declaringType))
            {
                isSimpleType = true;
            }

            yield return new DiscoveredResource(mi,
                                                resourceKey,
                                                translations,
                                                mi.Name,
                                                declaringType,
                                                returnType,
                                                isSimpleType,
                                                isResourceHidden)
            {
                TypeName = target.Name,
                TypeNamespace = target.Namespace,
                TypeOldName = oldResourceKeys.Item2,
                TypeOldNamespace = typeOldNamespace,
                OldResourceKey = oldResourceKeys.Item1
            };
        }

        private bool IsPropertyACollectionOfScalar(Type memberType)
        {
            var enumerableInterface = memberType.GetInterface(typeof(IEnumerable<>).FullName);
            return enumerableInterface != null && enumerableInterface.GenericTypeArguments.FirstOrDefault().IsSimpleType();
        }
    }
}
