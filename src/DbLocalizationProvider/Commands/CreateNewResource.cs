// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    public class CreateNewResource
    {
        public class Command : ICommand
        {
            public Command(string key, string userName, bool fromCode = true)
            {
                Key = key;
                UserName = userName;
                FromCode = fromCode;
            }

            public string Key { get; }

            public string UserName { get; }

            public bool FromCode { get; }
        }
    }
}
