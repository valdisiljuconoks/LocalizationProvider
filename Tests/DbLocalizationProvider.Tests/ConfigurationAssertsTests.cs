using System;
using System.Globalization;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class ConfigurationAssertsTests
    {
        [Fact]
        public void InvariantCannotBeDefaultLanguage()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ConfigurationContext.Setup(_ => _.DefaultResourceCulture = CultureInfo.InvariantCulture));
        }
    }
}
