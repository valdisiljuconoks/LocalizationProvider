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

        [Fact]
        public void ReplaceCommandHandler_ShouldReturnLast()
        {
            var sut = new TypeFactory();
            sut.ForCommand<SampleCommand>().SetHandler<SampleCommandHandler>();

            var result = sut.GetHandler(typeof(SampleCommand));

            Assert.True(result is SampleCommandHandler);

            // replacing handler
            sut.ForCommand<SampleCommand>().SetHandler<AnotherCommandQueryHandler>();

            result = sut.GetHandler(typeof(SampleCommand));

            Assert.True(result is AnotherCommandQueryHandler);
        }
    }
}
