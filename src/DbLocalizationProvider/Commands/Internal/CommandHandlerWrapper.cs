// Copyright © 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using DbLocalizationProvider.Abstractions;

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
            _inner.Execute((TCommand)command);
        }
    }
}
