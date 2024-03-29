using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests;

public class AnotherCommandQueryHandler : ICommandHandler<SampleCommand>
{
    public void Execute(SampleCommand command)
    {
        command.Field = "another set from handler";
    }
}
