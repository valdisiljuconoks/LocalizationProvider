using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbLocalizationProvider
{
    internal static class ResourceKeyBuilder
    {
        public static string BuildResourceKey(string prefix, MemberInfo mi, string separator = ".")
        {
            return BuildResourceKey(prefix, mi.Name);
        }

        public static string BuildResourceKey(string prefix, string name, string separator = ".")
        {
            return string.IsNullOrEmpty(prefix) ? name : $"{prefix}{separator}{name}";
        }

        public static string BuildResourceKey(string beginning, Stack<string> keyStack)
        {
            return keyStack.Aggregate(beginning, (prefix, name) => BuildResourceKey(prefix, name));
        }
    }
}
