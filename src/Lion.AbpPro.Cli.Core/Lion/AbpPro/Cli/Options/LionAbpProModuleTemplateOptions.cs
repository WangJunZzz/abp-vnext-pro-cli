namespace Lion.AbpPro.Cli;

public class LionAbpProModuleTemplateOptions
{
    public LionAbpProModuleTemplateGithubOptions Github { get; set; }

    public LionAbpProModuleTemplateReplaceOptions Replace { get; set; }
}

public class LionAbpProModuleTemplateGithubOptions
{
    public string Author { get; set; }

    public string RepositoryName { get; set; }

    public string Token { get; set; }
}

public class LionAbpProModuleTemplateReplaceOptions
{
    public string OldCompanyName { get; set; }

    public string OldProjectName { get; set; }
    
    public string OldModuleName { get; set; }

    /// <summary>
    /// 需要替换的文件
    /// </summary>
    public string ReplaceSuffix { get; set; }

    /// <summary>
    /// 需要排除的文件
    /// </summary>
    public string ExcludeFiles { get; set; }
}