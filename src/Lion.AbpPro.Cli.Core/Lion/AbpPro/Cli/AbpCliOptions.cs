﻿namespace Lion.AbpPro.Cli.Core.Lion.AbpPro.Cli;

public class AbpCliOptions
{
    public Dictionary<string, Type> Commands { get; }

    /// <summary>
    /// Default value: true.
    /// </summary>
    public bool CacheTemplates { get; set; } = true;

    /// <summary>
    /// Default value: "CLI".
    /// </summary>
    public string ToolName { get; set; } = "CLI";

    public AbpCliOptions()
    {
        Commands = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
    }
}