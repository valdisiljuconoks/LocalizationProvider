// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Commands.Internal
{
    internal abstract class CommandHandlerWrapper
    {
        public abstract Task Execute(ICommand command);
    }

    internal class CommandHandlerWrapper<TCommand> : CommandHandlerWrapper where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _inner;

        public CommandHandlerWrapper(ICommandHandler<TCommand> inner)
        {
            _inner = inner;
        }

        public override async Task Execute(ICommand command)
        {
            await _inner.Execute((TCommand)command);
        }
    }
}
