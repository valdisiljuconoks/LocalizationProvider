using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        public void Execute(SampleCommand command)
        {
            command.Field = "set from handler";
        }
    }
}
