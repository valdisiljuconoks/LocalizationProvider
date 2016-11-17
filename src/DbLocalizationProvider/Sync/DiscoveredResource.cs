using System;
using System.Diagnostics;
using System.Reflection;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Key: {Key}, Translation: {Translation}")]
    public class DiscoveredResource
    {
        public DiscoveredResource(MemberInfo info, string key, string translation, string propertyName, Type declaringType, Type returnType, bool isSimpleType)
        {
            Info = info;
            Key = key;
            Translation = translation;
            PropertyName = propertyName;
            DeclaringType = declaringType;
            ReturnType = returnType;
            IsSimpleType = isSimpleType;
        }

        public string Key { get; }

        public string Translation { get; }

        public string PropertyName { get; }

        public Type DeclaringType { get; }

        public Type ReturnType { get; }

        public bool IsSimpleType { get; }

        public MemberInfo Info { get; }

        public bool FromResourceKeyAttribute { get; set; }
    }
}
