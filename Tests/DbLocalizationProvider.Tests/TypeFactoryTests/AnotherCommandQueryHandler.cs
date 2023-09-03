using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class AnotherCommandQueryHandler : ICommandHandler<SampleCommand>
    {
        public Task Execute(SampleCommand command)
        {
            command.Field = "another set from handler";
            return Task.CompletedTask;
        }
    }
}
