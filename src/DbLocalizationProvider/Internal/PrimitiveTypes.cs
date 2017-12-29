// Copyright © 2017 Valdis Iljuconoks.
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
            if(_list.Any(x => x.IsAssignableFrom(type)))
            {
                return true;
            }

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.IsEnum;
        }
    }
}
