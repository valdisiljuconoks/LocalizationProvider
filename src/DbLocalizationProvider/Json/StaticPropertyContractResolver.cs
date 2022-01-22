// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace DbLocalizationProvider.Json
{
    /// <summary>
    /// Required to resolve static properties while deserializing from resource class
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Serialization.DefaultContractResolver" />
    public class StaticPropertyContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Gets the serializable members for the type.
        /// </summary>
        /// <param name="objectType">The type to get serializable members for.</param>
        /// <returns>
        /// The serializable members for the type.
        /// </returns>
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var baseMembers = base.GetSerializableMembers(objectType);
            var staticMembers = objectType.GetProperties(BindingFlags.Static | BindingFlags.Public);
            baseMembers.AddRange(staticMembers);

            return baseMembers;
        }
    }
}
