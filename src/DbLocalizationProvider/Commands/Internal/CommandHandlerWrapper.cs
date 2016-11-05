namespace DbLocalizationProvider.Commands.Internal
{
    internal abstract class CommandHandlerWrapper
    {
        public abstract void Execute(ICommand command);
    }

    internal class CommandHandlerWrapper<TCommand> : CommandHandlerWrapper where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _inner;

        public CommandHandlerWrapper(ICommandHandler<TCommand> inner)
        {
            _inner = inner;
        }

        public override void Execute(ICommand command)
        {
            _inner.Execute((TCommand) command);
        }
    }
}
