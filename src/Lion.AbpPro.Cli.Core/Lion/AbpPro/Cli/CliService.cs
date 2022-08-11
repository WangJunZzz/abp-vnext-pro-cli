using Lion.AbpPro.Cli.Args;
using Lion.AbpPro.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        Console.WriteLine("Lion ABP Pro CLI (https://https://doc.cncore.club/)");
        Console.WriteLine("请输入 lion.abp help 查看所有命令");
        
        //await _checkManager.CheckCliVersionAsync(LionAbpProCliConsts.PackageId);

        try
        {
            await RunInternalAsync();
        }

        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    private async Task RunInternalAsync()
    {
        string GetPromptInput()
        {
            return Console.ReadLine();
        }

        var promptInput = GetPromptInput();
        do
        {
            try
            {
                var commandArgs = promptInput.Split(" ").Where(x => !x.IsNullOrWhiteSpace() && x != _abpCliOptions.ToolName).ToArray();
                var commandLineArgs = _commandLineArgumentParser.Parse(commandArgs);

                var commandType = _commandSelector.Select(commandLineArgs);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var command = (IConsoleCommand)scope.ServiceProvider.GetRequiredService(commandType);
                    await command.ExecuteAsync(commandLineArgs);
                }
            }

            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            promptInput = GetPromptInput();
        } while (promptInput?.ToLower() != "exit");
    }
}