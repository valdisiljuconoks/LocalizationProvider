// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using EPiServer.Globalization;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.EPiServer.Queries;

/// <summary>
/// Determine default culture the Opti way
/// </summary>
public class EPiServerDetermineDefaultCulture
{
    /// <summary>
    /// What do you think this method (ctor) does, eh?
    /// </summary>
    public class Handler : IQueryHandler<DetermineDefaultCulture.Query, string>
    {
        private readonly ConfigurationContext _context;

        /// <summary>
        /// Create new instance of the command handler
        /// </summary>
        /// <param name="context">Configuration context</param>
        public Handler(IOptions<ConfigurationContext> context)
        {
            _context = context.Value;
        }

        /// <inheritdoc />
        public string Execute(DetermineDefaultCulture.Query query)
        {
            return _context.DefaultResourceCulture != null
                ? _context.DefaultResourceCulture.Name
                : ContentLanguage.PreferredCulture == null ? "en" : ContentLanguage.PreferredCulture.Name;
        }
    }
}
