// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider;

/// <summary>
/// The executor of commands.
/// </summary>
public class CommandExecutor : ICommandExecutor
{
    private readonly TypeFactory _factory;

    /// <summary>
    /// Creates new instance of the class.
    /// </summary>
    /// <param name="factory">Factory of the types.</param>
    public CommandExecutor(TypeFactory factory)
    {
        _factory = factory;
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

        var handler = _factory.GetCommandHandler(command);
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

        return _factory.GetCommandHandler(command) != null;
    }
}
