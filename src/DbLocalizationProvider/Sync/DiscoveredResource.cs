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
    }
}
