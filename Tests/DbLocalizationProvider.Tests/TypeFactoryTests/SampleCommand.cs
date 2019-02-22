using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class SampleCommand : ICommand
    {
        public string Field { get; set; }
    }
}
