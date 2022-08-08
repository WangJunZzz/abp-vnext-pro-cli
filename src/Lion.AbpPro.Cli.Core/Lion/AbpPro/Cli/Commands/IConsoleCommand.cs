namespace Lion.AbpPro.Cli.Core.Lion.AbpPro.Cli.Commands;

public interface IConsoleCommand
{
    Task ExecuteAsync(CommandLineArgs commandLineArgs);

    string GetUsageInfo();

    string GetShortDescription();
}