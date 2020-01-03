// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace DbLocalizationProvider.Json
{
    public class StaticPropertyContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var baseMembers = base.GetSerializableMembers(objectType);
            var staticMembers = objectType.GetProperties(BindingFlags.Static | BindingFlags.Public);
            baseMembers.AddRange(staticMembers);

            return baseMembers;
        }
    }
}
