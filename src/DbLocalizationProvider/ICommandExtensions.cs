// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    /// <summary>
    /// YES!
    /// </summary>
    public static class ICommandExtensions
    {
        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">command</exception>
        public static void Execute(this ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var handler = ConfigurationContext.Current.TypeFactory.GetCommandHandler(command);
            handler.Execute(command);
        }

        /// <summary>
        /// Checks whether this command could be executed
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns><c>true</c> if command has registered handler; <c>false</c> otherwise</returns>
        /// <exception cref="ArgumentNullException">command</exception>
        public static bool CanBeExecuted(this ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            return ConfigurationContext.Current.TypeFactory.GetCommandHandler(command) != null;
        }
    }
}
