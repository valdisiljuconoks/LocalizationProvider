using DbLocalizationProvider.Queries;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.TypeFactoryTests;

public class CommandTests
{
    private readonly CommandExecutor _sut;

    public CommandTests()
    {
        var ctx = new ConfigurationContext();
        ctx.TypeFactory
            .ForQuery<DetermineDefaultCulture.Query>()
            .SetHandler<DetermineDefaultCulture.Handler>()
            .ForCommand<SampleCommand>()
            .SetHandler<SampleCommandHandler>();

        _sut = new CommandExecutor(ctx.TypeFactory);
    }

    [Fact]
    public void ExecuteCommand()
    {
        var q = new SampleCommand();
        _sut.Execute(q);

        Assert.Equal("set from handler", q.Field);
    }

    [Fact]
    public void ReplaceCommandHandler_ShouldReturnLast()
    {
        var sut = new TypeFactory(new OptionsWrapper<ConfigurationContext>(new ConfigurationContext()));
        sut.ForCommand<SampleCommand>().SetHandler<SampleCommandHandler>();

        var result = sut.GetHandler(typeof(SampleCommand));

        Assert.True(result is SampleCommandHandler);

        // replacing handler
        sut.ForCommand<SampleCommand>().SetHandler<AnotherCommandQueryHandler>();

        result = sut.GetHandler(typeof(SampleCommand));

        Assert.True(result is AnotherCommandQueryHandler);
    }
}
