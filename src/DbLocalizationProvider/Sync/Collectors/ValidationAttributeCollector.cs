// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Refactoring;

namespace DbLocalizationProvider.Sync.Collectors
{
    internal class ValidationAttributeCollector : IResourceCollector
    {
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
            var keyAttributes = mi.GetCustomAttributes<ResourceKeyAttribute>().ToList();
            var validationAttributes = mi.GetCustomAttributes<ValidationAttribute>().ToList();

            if(keyAttributes.Count > 1 && validationAttributes.Any())
                throw new InvalidOperationException("Model with data annotation attributes cannot have more than one `[ResourceKey]` attribute.");

            foreach(var validationAttribute in validationAttributes)
            {
                if(validationAttribute.GetType() == typeof(DataTypeAttribute))
                    continue;

                if(keyAttributes.Any())
                    resourceKey = keyAttributes.First().Key;

                var validationResourceKey = ResourceKeyBuilder.BuildResourceKey(resourceKey, validationAttribute);
                var propertyName = validationResourceKey.Split('.').Last();

                var oldResourceKeys = OldResourceKeyBuilder.GenerateOldResourceKey(target, propertyName, mi, resourceKeyPrefix, typeOldName, typeOldNamespace);

                yield return new DiscoveredResource(mi,
                                                    validationResourceKey,
                                                    DiscoveredTranslation.FromSingle(string.IsNullOrEmpty(validationAttribute.ErrorMessage)
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
