namespace Lion.AbpPro.Cli;

public class LionAbpProOptions
{
    public LionAbpProGithubOptions Github { get; set; }

    public LionAbpProReplaceOptions Replace { get; set; }
}

public class LionAbpProGithubOptions
{
    public string Author { get; set; }

    public string RepositoryName { get; set; }

    public string Token { get; set; }
}

public class LionAbpProReplaceOptions
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

    /// <summary>
    /// 默认下载路径
    /// </summary>
    public string DownPath { get; set; }
}