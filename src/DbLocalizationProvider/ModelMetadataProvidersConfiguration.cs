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
using System.Linq.Expressions;

namespace DbLocalizationProvider
{
    public class ModelMetadataProvidersConfiguration
    {
        /// <summary>
        ///     Gets or sets a value to use cached version of ModelMetadataProvider.
        /// </summary>
        /// <value>
        ///     <c>true</c> if cached ModelMetadataProvider should be used; otherwise, <c>false</c>.
        /// </value>
        public bool UseCachedProviders { get; set; }

        /// <summary>
        ///     Gets or sets callback to call in order to enable ir disable legacy mode.
        ///     Legacy mode will ensure that if resource value starts with "/" symbol ModelMetadataProvider will try to look for
        ///     this XPath resource in localization provider collection once again.
        ///     This will make it possible to continue use *old* resource keys:
        ///     [DisplayName("/xpath/to/some/resource")]
        /// </summary>
        /// <value>
        ///     Return <c>true</c> to enable legacy mode translations.
        /// </value>
        public Func<bool> EnableLegacyMode { get; set; } = () => false;

        /// <summary>
        ///     Gets or sets a value to replace ModelMetadataProvider to use new db localization system.
        /// </summary>
        /// <value>
        ///     <c>true</c> if ModelMetadataProvider should be replaced; otherwise, <c>false</c>.
        /// </value>
        public bool ReplaceProviders { get; set; } = true;

        /// <summary>
        /// Set <c>true</c> to add translation returned from <see cref="RequiredFieldResource"/> for required fields.
        /// </summary>
        public bool MarkRequiredFields { get; set; } = false;

        /// <summary>
        /// If <see cref="MarkRequiredFields"/> is set to <c>true</c>, return of this method will be used to indicate required fields (added at the end of label).
        /// </summary>
        public Expression<Func<object>> RequiredFieldResource { get; set; }
    }
}
