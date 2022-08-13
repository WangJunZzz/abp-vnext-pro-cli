using Lion.AbpPro.Cli.Args;
using Lion.AbpPro.Cli.Commands;
using Lion.AbpPro.Cli.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli;

public class CliService : DomainService
{
    private readonly ICommandLineArgumentParser _commandLineArgumentParser;
    private readonly ICommandSelector _commandSelector;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly AbpCliOptions _abpCliOptions;
    private readonly CheckManager _checkManager;

    public CliService(ICommandLineArgumentParser commandLineArgumentParser,
        ICommandSelector commandSelector,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<AbpCliOptions> abpCliOptions,
        CheckManager checkManager)
    {
        _commandLineArgumentParser = commandLineArgumentParser;
        _commandSelector = commandSelector;
        _serviceScopeFactory = serviceScopeFactory;
        _checkManager = checkManager;
        _abpCliOptions = abpCliOptions.Value;
    }

    public async Task RunAsync(string[] args)
    {
        Logger.LogInformation("Lion ABP Pro CLI (https://https://doc.cncore.club/)");
        Logger.LogInformation("请输入 lion.abp help 查看所有命令");

        Logger.LogInformation($"1-{JsonConvert.SerializeObject(args)}");
        if (args == null || args.Length == 0 || args[0].ToLower() != _abpCliOptions.ToolName)
        {
            args = new[] { "help" };
        }
        else
        {
            args = args.Skip(1).ToArray();
        }

        //await _checkManager.CheckCliVersionAsync(LionAbpProCliConsts.PackageId);

        try
        {
            Logger.LogInformation($"2-{JsonConvert.SerializeObject(args)}");
            var commandLineArgs = _commandLineArgumentParser.Parse(args);
            Logger.LogInformation($"3-{JsonConvert.SerializeObject(commandLineArgs)}");
            await RunInternalAsync(commandLineArgs);
        }

        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    private async Task RunInternalAsync(CommandLineArgs commandLineArgs)
    {
        var commandType = _commandSelector.Select(commandLineArgs);
        Logger.LogInformation($"4-{JsonConvert.SerializeObject(commandType)}");
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var command = (IConsoleCommand)scope.ServiceProvider.GetRequiredService(commandType);
            Logger.LogInformation($"5-{JsonConvert.SerializeObject(command)}");
            await command.ExecuteAsync(commandLineArgs);
        }
    }
}