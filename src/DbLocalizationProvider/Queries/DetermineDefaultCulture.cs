// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
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
            /// <summary>
            /// Executes the command.
            /// </summary>
            /// <param name="query">This is the query instance</param>
            /// <returns>
            /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
            /// will.
            /// </returns>
            public string Execute(Query query)
            {
                return ConfigurationContext.Current.DefaultResourceCulture != null
                    ? ConfigurationContext.Current.DefaultResourceCulture.Name
                    : "en";
            }
        }
    }
}
