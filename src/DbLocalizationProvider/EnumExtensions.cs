// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    public static class EnumExtensions
    {
        public static string Translate(this Enum target, params object[] formatArguments)
        {
            return TranslateByCulture(target, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static string TranslateByCulture(this Enum target, CultureInfo culture, params object[] formatArguments)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (culture == null) throw new ArgumentNullException(nameof(culture));

            var resourceKey = ResourceKeyBuilder.BuildResourceKey(target.GetType(), target.ToString());
            return LocalizationProvider.Current.GetStringByCulture(resourceKey, culture, formatArguments);
        }
    }
}
