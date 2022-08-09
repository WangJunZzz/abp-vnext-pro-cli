using Lion.AbpPro.Cli.Args;
using Lion.AbpPro.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Lion.AbpPro.Cli;

public class CliService : ITransientDependency
{
    private readonly ILogger<CliService> _logger;
    private readonly ICommandLineArgumentParser _commandLineArgumentParser;
    private readonly ICommandSelector _commandSelector;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly AbpCliOptions _abpCliOptions;

    public CliService(ICommandLineArgumentParser commandLineArgumentParser, ILogger<CliService> logger, ICommandSelector commandSelector, IServiceScopeFactory serviceScopeFactory,
        IOptions<AbpCliOptions> abpCliOptions)
    {
        _commandLineArgumentParser = commandLineArgumentParser;
        _logger = logger;
        _commandSelector = commandSelector;
        _serviceScopeFactory = serviceScopeFactory;
        _abpCliOptions = abpCliOptions.Value;
    }

    public async Task RunAsync(string[] args)
    {
        _logger.LogInformation("Lion ABP Pro CLI (https://https://doc.cncore.club/)");
        _logger.LogInformation("请输入 lion-abp help 查看所有命令");
        try
        {
            await RunInternalAsync();
        }

        catch (Exception ex)
        {
            _logger.LogException(ex);
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
                _logger.LogException(ex);
            }

            promptInput = GetPromptInput();
        } while (promptInput?.ToLower() != "exit");
    }
}