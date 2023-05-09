using System;
using System.Linq;
using DbLocalizationProvider.Storage.SqlServer;
using Xunit;
using Xunit.Abstractions;

namespace DbLocalizationProvider.NetCore.Storage.SqlServer.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _outputHelper;

        public UnitTest1(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void Test1()
        {
            Settings.DbContextConnectionString = "Server=.;Database=loc-admin-ui;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=sample-user;Password=P@ssword$$";
            var repo = new ResourceRepository();

            var resource = repo.GetByKey("DbLocalizationProvider.AdminUI.AspNetCore.Resources.Title");

            _outputHelper.WriteLine(resource?.ResourceKey);
        }

        [Fact]
        public void Test2()
        {
            Settings.DbContextConnectionString = "Server=.;Database=loc-admin-ui;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=sample-user;Password=P@ssword$$";
            var repo = new ResourceRepository();

            var resource = repo.GetAll();

            _outputHelper.WriteLine(resource.Count().ToString());
        }

        [Fact]
        public void Test3()
        {
            Settings.DbContextConnectionString = "Server=.;Database=loc-admin-ui;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=sample-user;Password=P@ssword$$";
            var repo = new ResourceRepository();

            var resource = repo.Search("frontend", null, null, out var rowCount);

            _outputHelper.WriteLine(resource.Count().ToString());
            _outputHelper.WriteLine(rowCount.ToString());

        }
    }
}
