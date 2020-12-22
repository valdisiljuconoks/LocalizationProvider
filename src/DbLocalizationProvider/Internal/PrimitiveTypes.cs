// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;

namespace DbLocalizationProvider.Internal
{
    // http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive
    internal static class PrimitiveTypes
    {
        private static readonly Type[] _list;

        static PrimitiveTypes()
        {
            var types = new[]
            {
                typeof(Enum),
                typeof(string),
                typeof(char),
                typeof(Guid),
                typeof(bool),
                typeof(byte),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(sbyte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan)
            };

            var nullableTypes = from t in types
                where t.IsValueType
                select typeof(Nullable<>).MakeGenericType(t);

            _list = types.Concat(nullableTypes).ToArray();
        }

        public static bool IsSimpleType(this Type type)
        {
            if (_list.Any(x => x.IsAssignableFrom(type)))
            {
                return true;
            }

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.IsEnum;
        }
    }
}
