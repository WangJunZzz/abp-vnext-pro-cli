using Castle.Core.Configuration;
using Lion.AbpPro.Cli.Github;
using Lion.AbpPro.Cli.Replace;
using Lion.AbpPro.Cli.Zip;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli;

public class GenerateCodeManager : DomainService
{
    private readonly LionAbpProOptions _lionAbpProOptions;
    private readonly GithubManager _githubManager;
    private readonly ZipManager _zipManager;
    private readonly ReplaceManager _replaceManager;
    private readonly FileManager _fileManager;

    public GenerateCodeManager(
        IOptions<LionAbpProOptions> lionAbpProOptions,
        GithubManager githubManager,
        ZipManager zipManager,
        ReplaceManager replaceManager,
        FileManager fileManager)
    {
        _githubManager = githubManager;
        _zipManager = zipManager;
        _replaceManager = replaceManager;
        _fileManager = fileManager;
        _lionAbpProOptions = lionAbpProOptions.Value;
    }

    /// <summary>
    /// 生成Abp-Vnext-Pro
    /// </summary>
    /// <param name="companyName">公司名称</param>
    /// <param name="projectName">项目名称</param>
    /// <param name="version">版本</param>
    /// <param name="output">输出路径</param>
    public async Task LionAbpProAsync(
        string companyName,
        string projectName,
        string version,
        string output)
    {
        try
        {
            Logger.LogInformation($"正在获取{_lionAbpProOptions.Github.RepositoryName}...");
            var release = await _githubManager.GetReleaseVersionUrlAsync(_lionAbpProOptions.Github.Author, _lionAbpProOptions.Github.RepositoryName, _lionAbpProOptions.Github.Token, version);


            output = GetOutput(output, projectName);
            Logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}项目生成中...");
            // 源码下载路径
            var downFilePath = Path.Combine(CliPaths.AbpRootPath, _lionAbpProOptions.Github.RepositoryName, release.TagName) + ".zip";

            // 下载源码路径
            await _githubManager.DownloadAsync(release.DownloadUrl, downFilePath);

            // 解压路径
            var targetPath = downFilePath.Replace(".zip", "");

            _zipManager.ExtractZips(downFilePath, targetPath);


            //将解压之后的文件复制到输出目录
            _fileManager.CopyFolder(Path.Combine(targetPath, $"{_lionAbpProOptions.Github.RepositoryName}-{release.TagName}"), output, _lionAbpProOptions.Replace.ExcludeFiles);
            // 替换文件
            _replaceManager.ReplaceTemplates(output, _lionAbpProOptions.Replace.OldCompanyName, _lionAbpProOptions.Replace.OldProjectName, companyName, projectName,
                _lionAbpProOptions.Replace.ReplaceSuffix);
            
            Logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}生成成功.");
            Logger.LogInformation($"项目地址");
            Logger.LogInformation($"{output}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"程序异常：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取代码输出路径
    /// </summary>
    /// <param name="output">-o 用户输入地址</param>
    /// <param name="projectName">项目名称</param>
    /// <returns>string</returns>
    private string GetOutput(string output,
        string projectName)
    {
        if (output.IsNullOrWhiteSpace())
        {
            output = Path.Combine(Directory.GetCurrentDirectory(), projectName);
        }
        else
        {
            if (Path.IsPathRooted(output))
            {
                output = Path.Combine(output, projectName);
            }
            else
            {
                output = Path.Combine(Directory.GetCurrentDirectory(), output, projectName);
            }
        }

        return output;
    }
}