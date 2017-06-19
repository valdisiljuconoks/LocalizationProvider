using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Key: {Key}, Translations: {Translations.Count}")]
    public class DiscoveredResource
    {
        public DiscoveredResource(
            MemberInfo info,
            string key,
            List<DiscoveredTranslation> translations,
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

        public List<DiscoveredTranslation> Translations { get; }

        public string PropertyName { get; }

        public Type DeclaringType { get; }

        public Type ReturnType { get; }

        public bool IsSimpleType { get; }

        public MemberInfo Info { get; }

        public bool FromResourceKeyAttribute { get; set; }

        public bool IsHidden { get; set; }
    }
}
