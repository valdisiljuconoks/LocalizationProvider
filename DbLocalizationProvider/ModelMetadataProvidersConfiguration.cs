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

        public bool MarkRequiredFields { get; set; } = false;

        public Expression<Func<object>> RequiredFieldResource { get; set; }
    }
}
