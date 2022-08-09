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
    private readonly ILogger<GenerateCodeManager> _logger;
    private readonly LionAbpProOptions _lionAbpProOptions;
    private readonly GithubManager _githubManager;
    private readonly ZipManager _zipManager;
    private readonly ReplaceManager _replaceManager;
    private readonly FileManager _fileManager;

    public GenerateCodeManager(ILogger<GenerateCodeManager> logger, IOptions<LionAbpProOptions> lionAbpProOptions, GithubManager githubManager, ZipManager zipManager, ReplaceManager replaceManager,
        FileManager fileManager)
    {
        _logger = logger;
        _githubManager = githubManager;
        _zipManager = zipManager;
        _replaceManager = replaceManager;
        _fileManager = fileManager;
        _lionAbpProOptions = lionAbpProOptions.Value;
    }

    public async Task LionAbpProAsync(string companyName, string projectName, string version, string output)
    {
        try
        {
            _logger.LogInformation($"正在获取{_lionAbpProOptions.Github.RepositoryName}...");
            var release = await _githubManager.GetReleaseVersionUrlAsync(_lionAbpProOptions.Github.Author, _lionAbpProOptions.Github.RepositoryName, _lionAbpProOptions.Github.Token, version);
            _logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}下载中...");

            output = GetOutput(output, projectName);

            var downFilePath = Path.Combine(_lionAbpProOptions.Replace.DownPath, _lionAbpProOptions.Github.RepositoryName, release.TagName) + ".zip";
            // 下载源码
            await _githubManager.DownloadAsync(release.DownloadUrl, downFilePath);
            // 解压
            var targetPath = downFilePath.Replace(".zip", "");

            _zipManager.ExtractZips(downFilePath, targetPath);

            _logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}下载完成.");

            _fileManager.Copy(Path.Combine(targetPath, $"{_lionAbpProOptions.Github.RepositoryName}-{release.TagName}"), output);
            // 替换文件
            _replaceManager.ReplaceTemplates(
                output,
                _lionAbpProOptions.Replace.OldCompanyName,
                _lionAbpProOptions.Replace.OldProjectName,
                companyName,
                projectName,
                _lionAbpProOptions.Replace.ReplaceSuffix);
            _logger.LogInformation($"项目生成成功.");
            _logger.LogInformation($"生成项目地址：{output}");
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogError($"{ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"程序异常：{ex.Message}");
        }
    }

    private string GetOutput(string output, string projectName)
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
                output = Path.Combine(Directory.GetCurrentDirectory(), projectName);
            }
        }

        return output;
    }
}