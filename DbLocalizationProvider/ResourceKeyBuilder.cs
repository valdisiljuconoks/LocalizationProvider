using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbLocalizationProvider
{
    internal static class ResourceKeyBuilder
    {
        public static string BuildResourceKey(string prefix, MemberInfo mi)
        {
            return mi.GetCustomAttribute<ResourceKeyAttribute>() == null
                       ? $"{prefix}.{mi.Name}"
                       : mi.GetCustomAttribute<ResourceKeyAttribute>().Key;
        }

        public static string BuildResourceKey(string prefix, Stack<string> keyStack)
        {
            return keyStack.Aggregate(prefix, (current, memberName) => current + $".{memberName}");
        }
    }
}
