// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    public class DeleteResource
    {
        public class Command : ICommand
        {
            public Command(string key)
            {
                Key = key;
            }

            public string Key { get; }
        }
    }
}
