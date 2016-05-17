using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbLocalizationProvider
{
    internal static class ResourceKeyBuilder
    {
        public static string BuildResourceKey(string prefix, MemberInfo mi)
        {
            return BuildResourceKey(prefix, mi.Name);
        }

        public static string BuildResourceKey(string prefix, string name)
        {
            return string.IsNullOrEmpty(prefix) ? name : $"{prefix}.{name}";
        }

        public static string BuildResourceKey(string prefix, Stack<string> keyStack)
        {
            return keyStack.Aggregate(prefix, BuildResourceKey);
        }
    }
}
