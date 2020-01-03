// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    public static class ICommandExtensions
    {
        public static void Execute(this ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var handler = ConfigurationContext.Current.TypeFactory.GetCommandHandler(command);
            handler.Execute(command);
        }
    }
}
