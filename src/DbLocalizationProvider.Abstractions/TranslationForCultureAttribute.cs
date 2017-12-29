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
using System.Globalization;

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    /// If you wanna provide additional translations for the same resource for multiple languages, you gonna need this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class TranslationForCultureAttribute : Attribute
    {
        /// <summary>
        /// Obviously creates new instance of the attribute
        /// </summary>
        /// <param name="translation">Translation of the resource for given language.</param>
        /// <param name="culture">Language for the additional translation (will be used as argument for <see cref="CultureInfo"/>).</param>
        public TranslationForCultureAttribute(string translation, string culture)
        {
            Translation = translation;
            Culture = culture;
        }

        /// <summary>
        /// Translation of the resource for given language.
        /// </summary>
        public string Translation { get; }

        /// <summary>
        /// Language for the additional translation (will be used as argument for <see cref="CultureInfo"/>).
        /// </summary>
        public string Culture { get; }
    }
}
