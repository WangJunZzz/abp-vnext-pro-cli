using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli;

public abstract class GenerateCodeBase : DomainService
{
    /// <summary>
    /// 获取代码输出路径
    /// </summary>
    /// <param name="projectName">项目名称</param>
    /// <returns>string</returns>
    public string GetOutputPath(string projectName)
    {
        var path = Path.Combine(CliPaths.AbpRootPath, projectName);
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }

        Directory.CreateDirectory(path);
        return path;
    }
}