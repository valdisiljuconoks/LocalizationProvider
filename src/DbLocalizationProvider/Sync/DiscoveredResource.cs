// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Key: {Key}, Translations: {Translations.Count}")]
    public class DiscoveredResource
    {
        public DiscoveredResource(
            MemberInfo info,
            string key,
            ICollection<DiscoveredTranslation> translations,
            string propertyName,
            Type declaringType,
            Type returnType,
            bool isSimpleType,
            bool isHidden = false)
        {
            Info = info;
            Key = key;
            Translations = translations;
            PropertyName = propertyName;
            DeclaringType = declaringType;
            ReturnType = returnType;
            IsSimpleType = isSimpleType;
            IsHidden = isHidden;
        }

        public string Key { get; }

        public ICollection<DiscoveredTranslation> Translations { get; }

        public string PropertyName { get; }

        public Type DeclaringType { get; }

        public Type ReturnType { get; }

        public bool IsSimpleType { get; }

        public MemberInfo Info { get; }

        public bool FromResourceKeyAttribute { get; set; }

        public bool IsHidden { get; set; }

        public string TypeOldName { get; set; }

        public string TypeOldNamespace { get; set; }

        public string TypeName { get; set; }

        public string TypeNamespace { get; set; }

        public string OldResourceKey { get; set; }

        public bool IncludedExplicitly { get; set; }

        public bool IsIncluded()
        {
            return IsSimpleType || Info == null || Info.GetCustomAttribute<IncludeAttribute>() != null || IncludedExplicitly;
        }

        public bool IsComplex()
        {
            return !IsSimpleType && !IncludedExplicitly;
        }
    }
}
