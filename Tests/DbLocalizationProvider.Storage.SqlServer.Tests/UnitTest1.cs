using Xunit;

namespace DbLocalizationProvider.Storage.SqlServer.Tests
{
    public class UnitTest1
    {
        private readonly ResourceRepository _sut;

        public UnitTest1()
        {
            var ctx = new ConfigurationContext();
            _sut = new ResourceRepository(ctx);
        }

        [Fact]
        public void Test1()
        {
            Settings.DbContextConnectionString = "Server=.;Database=loc-admin-ui;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=sample-user;Password=P@ssword$$";
            var resource = _sut.GetByKey("DbLocalizationProvider.AdminUI.AspNetCore.Resources.Title");
        }

        [Fact]
        public void Test2()
        {
            Settings.DbContextConnectionString = "Server=.;Database=loc-admin-ui;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=sample-user;Password=P@ssword$$";
            var resource = _sut.GetAll();
        }
    }
}
