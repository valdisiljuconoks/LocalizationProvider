using System;
using System.Diagnostics;
using System.Reflection;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Key: {Key}, Translation: {Translation}")]
    public class DiscoveredResource
    {
        public DiscoveredResource(MemberInfo info, string key, string translation, Type declaringType, Type returnType, bool isSimpleType)
        {
            Info = info;
            Key = key;
            Translation = translation;
            DeclaringType = declaringType;
            ReturnType = returnType;
            IsSimpleType = isSimpleType;
        }

        public string Key { get; set; }

        public string Translation { get; set; }

        public Type DeclaringType { get; set; }

        public Type ReturnType { get; set; }

        public bool IsSimpleType { get; set; }

        public MemberInfo Info { get; set; }
    }
}
