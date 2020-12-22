// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    /// Handler of the <see cref="ICommand" />
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        void Execute(TCommand command);
    }
}
