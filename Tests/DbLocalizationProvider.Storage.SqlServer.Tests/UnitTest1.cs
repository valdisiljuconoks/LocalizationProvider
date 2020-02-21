using DbLocalizationProvider.Storage.SqlServer;
using Xunit;

namespace DbLocalizationProvider.NetCore.Storage.SqlServer.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Settings.DbContextConnectionString = "Server=.;Database=loc-admin-ui;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=sample-user;Password=P@ssword$$";
            var repo = new ResourceRepository();

            var resource = repo.GetByKey("DbLocalizationProvider.AdminUI.AspNetCore.Resources.Title");
        }

        [Fact]
        public void Test2()
        {
            Settings.DbContextConnectionString = "Server=.;Database=loc-admin-ui;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=sample-user;Password=P@ssword$$";
            var repo = new ResourceRepository();

            var resource = repo.GetAll();
        }
    }
}
