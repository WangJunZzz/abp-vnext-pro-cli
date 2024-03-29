﻿using Lion.AbpPro.Cli.Args;
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
        

        //await _checkManager.CheckCliVersionAsync(LionAbpProCliConsts.PackageId);

        try
        {
            var commandLineArgs = _commandLineArgumentParser.Parse(args);
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
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var command = (IConsoleCommand)scope.ServiceProvider.GetRequiredService(commandType);
            await command.ExecuteAsync(commandLineArgs);
        }
    }
}