// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    public class RemoveTranslation
    {
        public class Command : ICommand
        {
            public Command(string key, CultureInfo language)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Language = language ?? throw new ArgumentNullException(nameof(language));
            }

            public string Key { get; }

            public CultureInfo Language { get; }
        }
    }
}
