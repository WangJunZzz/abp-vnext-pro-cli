using Lion.AbpPro.Cli.Core.Lion.AbpPro.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Lion.AbpPro.Cli.Core.Lion.AbpPro.Cli;

public class CliService:ITransientDependency
{
    private readonly ILogger<CliService> _logger;
    private readonly ICommandLineArgumentParser _commandLineArgumentParser;
    private readonly ICommandSelector _commandSelector;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public CliService(ICommandLineArgumentParser commandLineArgumentParser, ILogger<CliService> logger, ICommandSelector commandSelector, IServiceScopeFactory serviceScopeFactory)
    {
        _commandLineArgumentParser = commandLineArgumentParser;
        _logger = logger;
        _commandSelector = commandSelector;
        _serviceScopeFactory = serviceScopeFactory;
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
                var commandLineArgs = _commandLineArgumentParser.Parse(promptInput.Split(" ").Where(x => !x.IsNullOrWhiteSpace()).ToArray());

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