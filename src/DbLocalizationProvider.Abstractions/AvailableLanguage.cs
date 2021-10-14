// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    /// Model to represent available language (most for AdminUI)
    /// </summary>
    public class AvailableLanguage
    {
        /// <summary>
        /// What do you think will do? Obviously create new instance of the class.
        /// </summary>
        /// <param name="displayName">Display name of the language (might be different from Name or EnglishName of the CultureInfo set for the available language).</param>
        /// <param name="sortIndex">If we need sorting, this is the index to use.</param>
        /// <param name="cultureInfo">Actual culture info for the language.</param>
        public AvailableLanguage(string displayName, int sortIndex, CultureInfo cultureInfo)
        {
            DisplayName = displayName;
            SortIndex = sortIndex;
            CultureInfo = cultureInfo;
        }

        /// <summary>
        /// Display name of the language (might be different from Name or EnglishName of the CultureInfo set for the available language).
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// If we need sorting, this is the index to use.
        /// </summary>
        public int SortIndex { get; }

        /// <summary>
        /// Actual culture info for the language.
        /// </summary>
        public CultureInfo CultureInfo { get; }
    }
}
