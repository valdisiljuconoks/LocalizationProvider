// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Storage.SqlServer
{
    /// <summary>
    /// Reads all resources from underlying storage
    /// </summary>
    public class GetAllResourcesHandler : IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>>
    {
        /// <summary>
        /// Place where query handling happens
        /// </summary>
        /// <param name="query">This is the query instance</param>
        /// <returns>
        /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
        /// will.
        /// </returns>
        public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
        {
            var repository = new ResourceRepository();

            return repository.GetAll();
        }
    }
}
