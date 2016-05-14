using System;
using System.Diagnostics;
using System.Reflection;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Key: {Key}, Translation: {Translation}")]
    public class DiscoveredResource
    {
        public DiscoveredResource(MemberInfo info, string key, string translation, Type declaringType, Type returnType)
        {
            Info = info;
            Key = key;
            Translation = translation;
            DeclaringType = declaringType;
            ReturnType = returnType;
        }

        public string Key { get; set; }

        public string Translation { get; set; }

        public Type DeclaringType { get; set; }

        public Type ReturnType { get; set; }

        public MemberInfo Info { get; set; }
    }
}
