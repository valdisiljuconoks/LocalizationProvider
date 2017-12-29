using DbLocalizationProvider.Abstractions;
using Xunit;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class CommandTests
    {
        public CommandTests()
        {
            ConfigurationContext.Current.TypeFactory.ForCommand<SampleCommand>().SetHandler<SampleCommandHandler>();
        }

        [Fact]
        public void ExecuteCommand()
        {
            var q = new SampleCommand();
            q.Execute();

            Assert.Equal("set from handler", q.Field);
        }
    }

    public class SampleCommand : ICommand
    {
        public string Field { get; set; }
    }

    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        public void Execute(SampleCommand command)
        {
            command.Field = "set from handler";
        }
    }
}
