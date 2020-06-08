using Xunit;

namespace DbLocalizationProvider.Storage.PostgreSql.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Settings.DbContextConnectionString = "Host=localhost;Database=BTP;Username=postgres;Password=Only4Tests;;MaxPoolSize=80;";
         
            var schema = new SchemaUpdater();
            schema.Execute(null);
            
            var repo = new ResourceRepository();

            var resource = repo.GetByKey("DbLocalizationProvider.AdminUI.AspNetCore.Resources.Title");
        }

        [Fact]
        public void Test2()
        {
            Settings.DbContextConnectionString = "Host=localhost;Database=BTP;Username=postgres;Password=Only4Tests;;MaxPoolSize=80;";
            var repo = new ResourceRepository();

            var resource = repo.GetAll();
        }
    }
}
