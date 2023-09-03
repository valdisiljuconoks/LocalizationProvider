// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Refactoring;

namespace DbLocalizationProvider.Sync.Collectors
{
    internal class ValidationAttributeCollector : IResourceCollector
    {
        private readonly ResourceKeyBuilder _keyBuilder;
        private readonly OldResourceKeyBuilder _oldKeyBuilder;
        private readonly DiscoveredTranslationBuilder _translationBuilder;

        public ValidationAttributeCollector(
            ResourceKeyBuilder keyBuilder,
            OldResourceKeyBuilder oldKeyBuilder,
            DiscoveredTranslationBuilder translationBuilder)
        {
            _keyBuilder = keyBuilder;
            _oldKeyBuilder = oldKeyBuilder;
            _translationBuilder = translationBuilder;
        }

        public async IAsyncEnumerable<DiscoveredResource> GetDiscoveredResources(
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
            var keyAttributes = mi.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            var validationAttributes = mi.GetCustomAttributes<ValidationAttribute>().ToList();

            foreach (var validationAttribute in validationAttributes)
            {
                if (validationAttribute.GetType() == typeof(DataTypeAttribute))
                {
                    continue;
                }

                if (keyAttributes.Count > 1)
                {
                    continue;
                }

                if (keyAttributes.Count == 1)
                {
                    resourceKey = keyAttributes[0].Key;
                }

                var validationResourceKey = _keyBuilder.BuildResourceKey(resourceKey, validationAttribute);
                var propertyName = validationResourceKey.Split('.').Last();

                var oldResourceKeys =
                    _oldKeyBuilder.GenerateOldResourceKey(
                        target,
                        propertyName,
                        mi,
                        resourceKeyPrefix,
                        typeOldName,
                        typeOldNamespace);

                yield return new DiscoveredResource(
                    mi,
                    validationResourceKey,
                    await _translationBuilder.FromSingle(string.IsNullOrEmpty(validationAttribute.ErrorMessage)
                                                             ? propertyName
                                                             : validationAttribute.ErrorMessage),
                    propertyName,
                    declaringType,
                    returnType,
                    isSimpleType)
                {
                    TypeName = target.Name,
                    TypeNamespace = target.Namespace,
                    TypeOldName = oldResourceKeys.Item2,
                    TypeOldNamespace = typeOldNamespace,
                    OldResourceKey = oldResourceKeys.Item1
                };
            }
        }
    }
}
