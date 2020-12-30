// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    /// <summary>
    /// The executor of commands.
    /// </summary>
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ConfigurationContext _configurationContext;

        /// <summary>
        ///Creates new instance of the class.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        public CommandExecutor(ConfigurationContext configurationContext)
        {
            _configurationContext = configurationContext;
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">command</exception>
        public void Execute(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = _configurationContext.TypeFactory.GetCommandHandler(command, _configurationContext);
            handler.Execute(command);
        }

        /// <summary>
        /// Checks whether this command could be executed
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns><c>true</c> if command has registered handler; <c>false</c> otherwise</returns>
        /// <exception cref="ArgumentNullException">command</exception>
        public bool CanBeExecuted(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return _configurationContext.TypeFactory.GetCommandHandler(command, _configurationContext) != null;
        }
    }
}
