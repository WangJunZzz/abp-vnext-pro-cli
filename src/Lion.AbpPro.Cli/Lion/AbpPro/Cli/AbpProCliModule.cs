using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Lion.AbpPro.Cli;

[DependsOn(
    typeof(Lion.AbpPro.Cli.AbpProCliCoreModule),
    typeof(AbpAutofacModule)
)]
public class AbpProCliModule : AbpModule
{
    // public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    // {
    //     var logger = context.ServiceProvider.GetRequiredService<ILogger<CliModule>>();
    //     var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
    //     logger.LogInformation($"MySettingName => {configuration["MySettingName"]}");
    //
    //     var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
    //     logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");
    //
    //     return Task.CompletedTask;
    // }
}
