using Lion.AbpPro.Cli.Args;

namespace Lion.AbpPro.Cli.Commands;

public interface IConsoleCommand
{
    Task ExecuteAsync(CommandLineArgs commandLineArgs);

    string GetUsageInfo();

    string GetShortDescription();
}