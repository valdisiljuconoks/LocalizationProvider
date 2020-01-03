// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    public static class ListOfCultureInfoExtensions
    {
        public static List<CultureInfo> Try(this List<CultureInfo> list, CultureInfo fallbackLanguage)
        {
            if (fallbackLanguage == null) throw new ArgumentNullException(nameof(fallbackLanguage));

            list.Add(fallbackLanguage);

            return list;
        }

        public static List<CultureInfo> Try(this List<CultureInfo> list, IList<CultureInfo> fallbackLanguages)
        {
            if (fallbackLanguages == null) throw new ArgumentNullException(nameof(fallbackLanguages));

            fallbackLanguages.ForEach(list.Add);

            return list;
        }

        public static List<CultureInfo> Then(this List<CultureInfo> list, CultureInfo fallbackLanguage)
        {
            return list.Try(fallbackLanguage);
        }
    }
}
