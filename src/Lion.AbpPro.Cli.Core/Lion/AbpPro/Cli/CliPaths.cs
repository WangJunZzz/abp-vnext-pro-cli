namespace Lion.AbpPro.Cli.Core.Lion.AbpPro.Cli;

public class CliPaths
{
    public static string Log => Path.Combine(AbpRootPath, "cli", "logs");
    
    public static readonly string AbpRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".abp");
}