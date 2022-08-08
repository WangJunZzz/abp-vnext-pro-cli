using System;
using System.IO;
using System.Threading.Tasks;
using Lion.AbpPro.Cli.Core.Lion.AbpPro.Cli;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Volo.Abp;

namespace Lion.AbpPro.Cli;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp.IdentityModel", LogEventLevel.Information)
#if DEBUG
            .MinimumLevel.Override("Volo.Abp.Cli", LogEventLevel.Debug)
#else
            .MinimumLevel.Override("Volo.Abp.Cli", LogEventLevel.Information)
#endif
            .Enrich.FromLogContext()
            .WriteTo.File(Path.Combine(CliPaths.Log, "lion-abp-pro-cli-logs.txt"))
            .WriteTo.Console()
            .CreateLogger();

        using (var application = AbpApplicationFactory.Create<AbpProCliModule>(
                   options =>
                   {
                       options.UseAutofac();
                       options.Services.AddLogging(c => c.AddSerilog());
                   }))
        {
            application.Initialize();

            await application.ServiceProvider
                .GetRequiredService<CliService>()
                .RunAsync(args);

            application.Shutdown();

            Log.CloseAndFlush();
        }
    }
}
