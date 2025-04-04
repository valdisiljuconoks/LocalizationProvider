// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Queries;

/// <summary>
/// Which is the default language? With help of this command you can get to know this magic.
/// </summary>
public class DetermineDefaultCulture
{
    /// <summary>
    /// Which is the default language? With help of this command you can get to know this magic.
    /// </summary>
    public class Query : IQuery<string> { }

    /// <summary>
    /// Default handler to answer question about which is the default language.
    /// This handler is reading <see cref="ConfigurationContext.DefaultResourceCulture" /> property.
    /// </summary>
    public class Handler : IQueryHandler<Query, string>
    {
        private const string TheDefaultCulture = "en";
        private readonly IOptions<ConfigurationContext> _context;

        /// <summary>
        /// Creates new instance of the handler.
        /// </summary>
        /// <param name="context">Configuration context.</param>
        public Handler(IOptions<ConfigurationContext> context)
        {
            _context = context;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="query">This is the query instance</param>
        /// <returns>
        /// You have to return something from the query execution. Of course, you can return <c>null</c> as well if you
        /// will.
        /// </returns>
        public string Execute(Query query)
        {
            return _context.Value.DefaultResourceCulture != null
                ? _context.Value.DefaultResourceCulture.Name
                : TheDefaultCulture;
        }
    }
}
