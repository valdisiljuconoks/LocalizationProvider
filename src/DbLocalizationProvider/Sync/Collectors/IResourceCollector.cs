// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync.Collectors
{
    /// <summary>
    /// Interface for implementing custom resource collector
    /// </summary>
    public interface IResourceCollector
    {
        /// <summary>
        /// Gets the discovered resources.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="mi">The mi.</param>
        /// <param name="translation">The translation.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="resourceKeyPrefix">The resource key prefix.</param>
        /// <param name="typeKeyPrefixSpecified">if set to <c>true</c> [type key prefix specified].</param>
        /// <param name="isHidden">if set to <c>true</c> [is hidden].</param>
        /// <param name="typeOldName">Old name of the type.</param>
        /// <param name="typeOldNamespace">The type old namespace.</param>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="isSimpleType">if set to <c>true</c> [is simple type].</param>
        /// <returns></returns>
        IAsyncEnumerable<DiscoveredResource> GetDiscoveredResources(
            Type target,
            object instance,
            MemberInfo mi,
            string translation,
            string resourceKey,
            string resourceKeyPrefix,
            bool typeKeyPrefixSpecified,
            bool isHidden,
            string typeOldName,
            string typeOldNamespace,
            Type declaringType,
            Type returnType,
            bool isSimpleType);
    }
}
