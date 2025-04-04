// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands.Internal;

internal abstract class CommandHandlerWrapper
{
    public abstract void Execute(ICommand command);
}

internal class CommandHandlerWrapper<TCommand>(ICommandHandler<TCommand> inner) : CommandHandlerWrapper
    where TCommand : ICommand
{
    public override void Execute(ICommand command)
    {
        inner.Execute((TCommand)command);
    }
}
