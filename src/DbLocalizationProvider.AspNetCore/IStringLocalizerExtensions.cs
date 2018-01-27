// Copyright (c) 2018 Valdis Iljuconoks.
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
using System.Globalization;
using System.Linq.Expressions;
using DbLocalizationProvider.Internal;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public static class IStringLocalizerExtensions
    {
        public static LocalizedString GetString(this IStringLocalizer target, Expression<Func<object>> model, params object[] formatArguments)
        {
            return target[ExpressionHelper.GetFullMemberName(model), formatArguments];
        }

        public static LocalizedString GetStringByCulture(this IStringLocalizer target, Expression<Func<object>> model, CultureInfo language, params object[] formatArguments)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if(language == null)
                throw new ArgumentNullException(nameof(language));

            return target.WithCulture(language)[ExpressionHelper.GetFullMemberName(model), formatArguments];
        }

        public static LocalizedString GetString<T>(this IStringLocalizer<T> target, Expression<Func<T, object>> model, params object[] formatArguments)
        {
            return target[ExpressionHelper.GetFullMemberName(model), formatArguments];
        }

        public static LocalizedString GetStringByCulture<T>(
            this IStringLocalizer<T> target,
            Expression<Func<T, object>> model,
            CultureInfo language,
            params object[] formatArguments)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if(language == null)
                throw new ArgumentNullException(nameof(language));

            return target.WithCulture(language)[ExpressionHelper.GetFullMemberName(model), formatArguments];
        }
    }
}
