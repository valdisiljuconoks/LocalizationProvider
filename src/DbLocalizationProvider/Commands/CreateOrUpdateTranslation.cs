// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands
{
    public class CreateOrUpdateTranslation
    {
        public class Command : ICommand
        {
            public Command(string key, CultureInfo language, string translation)
            {
                Key = key;
                Language = language;
                Translation = translation;
            }

            public string Key { get; }

            public CultureInfo Language { get; }

            public string Translation { get; }
        }
    }
}
