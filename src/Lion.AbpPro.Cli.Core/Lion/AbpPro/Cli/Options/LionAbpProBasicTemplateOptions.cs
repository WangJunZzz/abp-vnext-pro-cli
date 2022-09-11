namespace Lion.AbpPro.Cli;

public class LionAbpProBasicTemplateOptions
{
    public LionAbpProBasicTemplateGithubOptions Github { get; set; }

    public LionAbpProBasicTemplateReplaceOptions Replace { get; set; }
}

public class LionAbpProBasicTemplateGithubOptions
{
    public string Author { get; set; }

    public string RepositoryName { get; set; }

    public string Token { get; set; }
}

public class LionAbpProBasicTemplateReplaceOptions
{
    public string OldCompanyName { get; set; }

    public string OldProjectName { get; set; }

    /// <summary>
    /// 需要替换的文件
    /// </summary>
    public string ReplaceSuffix { get; set; }

    /// <summary>
    /// 需要排除的文件
    /// </summary>
    public string ExcludeFiles { get; set; }
}