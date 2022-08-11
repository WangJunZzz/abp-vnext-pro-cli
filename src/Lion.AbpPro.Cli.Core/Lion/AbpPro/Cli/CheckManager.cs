using System.Reflection;
using Lion.AbpPro.Cli.NuGet;
using Lion.AbpPro.Cli.Utils;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli;

public class CheckManager : DomainService
{
    private readonly NuGetService _nuGetService;
    private readonly CmdHelper _cmdHelper;

    public CheckManager(
        NuGetService nuGetService,
        CmdHelper cmdHelper)
    {
        _nuGetService = nuGetService;
        _cmdHelper = cmdHelper;
    }

    /// <summary>
    /// 检查Nuget包版本
    /// </summary>
    /// <param name="packageId">包名</param>
    public async Task CheckCliVersionAsync(string packageId)
    {
        var assembly = typeof(CliService).Assembly;
        var currentCliVersion = await GetCurrentCliVersionInternalAsync(assembly);
        if (currentCliVersion.IsNullOrWhiteSpace()) return;
        Logger.LogInformation($"{packageId} 当前版本 {currentCliVersion}");

        try
        {
            var latestVersion = await _nuGetService.GetLatestVersionOrNullAsync(packageId);
            if (latestVersion.IsNullOrWhiteSpace()) return;
            var currentCliVersionInt = Convert.ToInt32(currentCliVersion.Split('.', StringSplitOptions.RemoveEmptyEntries).ToList().JoinAsString(""));
            var latestVersionInt = Convert.ToInt32(latestVersion.Split('.', StringSplitOptions.RemoveEmptyEntries).ToList().JoinAsString(""));
            if (latestVersionInt > currentCliVersionInt)
            {
                Logger.LogWarning($"检测到{packageId}最新版本:{latestVersion}");
                Logger.LogWarning($"dotnet tool update -g {packageId}");
            }
        }
        catch (Exception e)
        {
            Logger.LogWarning("检查 Lion.AbpPro.Cli 版本失败.");
            Logger.LogWarning(e.Message);
        }
    }

    private string GetToolPath(Assembly assembly)
    {
        if (!assembly.Location.Contains(".store"))
        {
            return null;
        }

        return assembly.Location.Substring(0, assembly.Location.IndexOf(".store", StringComparison.Ordinal));
    }


    private async Task<string> GetCurrentCliVersionInternalAsync(Assembly assembly)
    {
        string currentCliVersion = null;

        var consoleOutput = new StringReader(_cmdHelper.RunCmdAndGetOutput($"dotnet tool list -g", out int exitCode));
        string line;
        while ((line = await consoleOutput.ReadLineAsync()) != null)
        {
            if (line.StartsWith(LionAbpProCliConsts.PackageId.ToLower(), StringComparison.InvariantCultureIgnoreCase))
            {
                currentCliVersion = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries)[1];
                break;
            }
        }


        return currentCliVersion;
    }
}