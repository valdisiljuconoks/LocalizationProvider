using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DbLocalizationProvider.Core.AspNetSample;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args)
            .Build()
            .Run();
    }

    public static IHostBuilder BuildWebHost(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStaticWebAssets();
                webBuilder.UseStartup<Startup>();
            });
    }
}
