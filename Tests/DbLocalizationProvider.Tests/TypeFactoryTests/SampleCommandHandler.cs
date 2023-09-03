using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        public Task Execute(SampleCommand command)
        {
            command.Field = "set from handler";
            return Task.CompletedTask;
        }
    }
}
