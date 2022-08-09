namespace Lion.AbpPro.Cli.Args;

public static class CommandOptions
{
    /// <summary>
    ///  公司名称
    /// </summary>
    public static class Company
    {
        public const string Short = "c";
        public const string Long = "company";
    }

    /// <summary>
    /// 项目名称
    /// </summary>
    public static class Project
    {
        public const string Short = "p";
        public const string Long = "project";
    }


    /// <summary>
    /// 输出目录
    /// </summary>
    public static class OutputFolder
    {
        public const string Short = "o";
        public const string Long = "output";
    }

    /// <summary>
    /// 版本
    /// </summary>
    public static class Version
    {
        public const string Short = "v";
        public const string Long = "version";
    }
}