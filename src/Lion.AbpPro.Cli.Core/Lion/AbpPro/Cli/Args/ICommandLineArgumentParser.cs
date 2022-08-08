namespace Lion.AbpPro.Cli.Core.Lion.AbpPro.Cli.Commands;

public interface ICommandLineArgumentParser
{
    CommandLineArgs Parse(string[] args);

    CommandLineArgs Parse(string lineText);
}