// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    /// <summary>
    /// Retrieves single resource
    /// </summary>
    public class GetResourceHandler : IQueryHandler<GetResource.Query, LocalizationResource>
    {
        private readonly IResourceRepository _repository;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="repository">Resource repository</param>
        public GetResourceHandler(IResourceRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Place where query handling happens
        /// </summary>
        /// <param name="query">This is the query instance</param>
        /// <returns>
        /// You have to return something from the query execution. Of course you can return <c>null</c> as well if you
        /// will.
        /// </returns>
        public LocalizationResource Execute(GetResource.Query query)
        {
            return _repository.GetByKey(query.ResourceKey);
        }
    }
}
