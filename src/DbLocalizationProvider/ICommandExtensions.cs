using System;

namespace DbLocalizationProvider
{
    public static class ICommandExtensions
    {
        public static void Execute(this ICommand command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            var handler = ConfigurationContext.Current.TypeFactory.GetCommandHandler(command);
            handler.Execute(command);
        }
    }
}
