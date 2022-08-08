using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Lion.AbpPro.Cli.Core.Lion.AbpPro.Cli.Commands;

public class HelpCommand: IConsoleCommand, ITransientDependency
{
  public const string Name = "help";
    
    public ILogger<HelpCommand> Logger { get; set; }
    protected AbpCliOptions AbpCliOptions { get; }
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    public HelpCommand(IOptions<AbpCliOptions> cliOptions,
        IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Logger = NullLogger<HelpCommand>.Instance;
        AbpCliOptions = cliOptions.Value;
    }

    public Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (string.IsNullOrWhiteSpace(commandLineArgs.Target))
        {
            Logger.LogInformation(GetUsageInfo());
            return Task.CompletedTask;
        }

        if (!AbpCliOptions.Commands.ContainsKey(commandLineArgs.Target))
        {
            Logger.LogWarning($"There is no command named {commandLineArgs.Target}.");
            Logger.LogInformation(GetUsageInfo());
            return Task.CompletedTask;
        }

        var commandType = AbpCliOptions.Commands[commandLineArgs.Target];

        using (var scope = ServiceScopeFactory.CreateScope())
        {
            var command = (IConsoleCommand)scope.ServiceProvider.GetRequiredService(commandType);
            Logger.LogInformation(command.GetUsageInfo());
        }

        return Task.CompletedTask;
    }

    public string GetUsageInfo()
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("指引:");
        sb.AppendLine("    lion-abp <command> <target> [options]");
        sb.AppendLine("命令列表:");

        foreach (var command in AbpCliOptions.Commands.ToArray())
        {
            string shortDescription;

            using (var scope = ServiceScopeFactory.CreateScope())
            {
                shortDescription = ((IConsoleCommand)scope.ServiceProvider
                        .GetRequiredService(command.Value)).GetShortDescription();
            }

            sb.Append("    > ");
            sb.Append(command.Key);
            sb.Append(string.IsNullOrWhiteSpace(shortDescription) ? "" : ":");
            sb.Append(" ");
            sb.AppendLine(shortDescription);
        }

 
        sb.AppendLine("查看命令帮助");
        sb.AppendLine("    lion-abp help <command>");

        return sb.ToString();
    }

    public string GetShortDescription()
    {
        return "Show command line help. Write ` abp help <command> `";
    }
}