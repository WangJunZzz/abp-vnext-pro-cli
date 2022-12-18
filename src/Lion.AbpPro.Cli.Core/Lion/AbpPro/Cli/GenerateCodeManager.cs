using Castle.Core.Configuration;
using Lion.AbpPro.Cli.Cryptography;
using Lion.AbpPro.Cli.Github;
using Lion.AbpPro.Cli.Replace;
using Lion.AbpPro.Cli.Zip;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli;

public class GenerateCodeManager : GenerateCodeBase
{
    private readonly LionAbpProOptions _lionAbpProOptions;
    private readonly LionAbpProBasicTemplateOptions _lionAbpProBasicTemplateOptions;
    private readonly LionAbpProBasicNoOcelotTemplateOptions _lionAbpProBasicNoOcelotTemplateOptions;
    private readonly LionAbpProModuleTemplateOptions _lionAbpProModuleTemplateOptions;
    private readonly GithubManager _githubManager;
    private readonly ZipManager _zipManager;
    private readonly ReplaceManager _replaceManager;
    private readonly FileManager _fileManager;

    public GenerateCodeManager(
        IOptions<LionAbpProOptions> lionAbpProOptions,
        GithubManager githubManager,
        ZipManager zipManager,
        ReplaceManager replaceManager,
        FileManager fileManager,
        IOptions<LionAbpProBasicTemplateOptions> lionAbpProBasicTemplateOptions,
        IOptions<LionAbpProBasicNoOcelotTemplateOptions> lionAbpProBasicNoOcelotTemplateOptions,
        IOptions<LionAbpProModuleTemplateOptions> lionAbpProModuleTemplateOptions)
    {
        _githubManager = githubManager;
        _zipManager = zipManager;
        _replaceManager = replaceManager;
        _fileManager = fileManager;
        _lionAbpProModuleTemplateOptions = lionAbpProModuleTemplateOptions.Value;
        _lionAbpProBasicNoOcelotTemplateOptions = lionAbpProBasicNoOcelotTemplateOptions.Value;
        _lionAbpProBasicTemplateOptions = lionAbpProBasicTemplateOptions.Value;
        _lionAbpProOptions = lionAbpProOptions.Value;
    }

    /// <summary>
    /// 生成Abp-Vnext-Pro
    /// </summary>
    /// <param name="companyName">公司名称</param>
    /// <param name="projectName">项目名称</param>
    /// <param name="version">版本</param>
    public async Task LionAbpProAsync(
        string companyName,
        string projectName,
        string version)
    {
        try
        {
            Logger.LogInformation($"读取{_lionAbpProOptions.Github.RepositoryName}版本信息...");
            //var token = RSA.Decrypt(_lionAbpProOptions.Github.Token, LionAbpProCliConsts.LionAbpPro);
            var token = Token.Decrypt(_lionAbpProOptions.Github.Token);
            var release = await _githubManager.GetReleaseVersionUrlAsync(_lionAbpProOptions.Github.Author, _lionAbpProOptions.Github.RepositoryName, token, version);

            Logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}版本:{release.TagName}.");
            var outputPath = GetOutputPath(projectName);

            // 源码下载路径
            var downFilePath = Path.Combine(CliPaths.AbpRootPath, _lionAbpProOptions.Github.RepositoryName, release.TagName) + ".zip";
            Logger.LogInformation($"正在下载{_lionAbpProOptions.Github.RepositoryName}源码...");
            // 下载源码路径
            await _githubManager.DownloadAsync(release.DownloadUrl, downFilePath);
            Logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}下载完成.");
            // 解压路径
            var targetPath = downFilePath.Replace(".zip", "");
            Logger.LogInformation($"正在解压{_lionAbpProOptions.Github.RepositoryName}...");
            _zipManager.ExtractZips(downFilePath, targetPath);
            Logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}解压完成.");

            Logger.LogInformation($"正在生成{_lionAbpProOptions.Github.RepositoryName}...");
            //将解压之后的文件复制到输出目录
            _fileManager.CopyFolder(Path.Combine(targetPath, $"{_lionAbpProOptions.Github.RepositoryName}-{release.TagName}"), outputPath, _lionAbpProOptions.Replace.ExcludeFiles);
            // 替换文件
            _replaceManager.ReplaceTemplates(outputPath, _lionAbpProOptions.Replace.OldCompanyName, _lionAbpProOptions.Replace.OldProjectName, companyName, projectName,
                _lionAbpProOptions.Replace.ReplaceSuffix);

            Logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}生成成功.");
            Logger.LogInformation($"{_lionAbpProOptions.Github.RepositoryName}输出路径:{outputPath}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"程序异常：{ex.Message}");
        }
    }

    /// <summary>
    /// 生成Abp-Vnext-Pro-Basic
    /// </summary>
    /// <param name="companyName">公司名称</param>
    /// <param name="projectName">项目名称</param>
    /// <param name="version">版本</param>
    public async Task LionAbpProBasicAsync(
        string companyName,
        string projectName,
        string version)
    {
        try
        {
            Logger.LogInformation($"读取{_lionAbpProBasicTemplateOptions.Github.RepositoryName}版本信息...");

            var token = Token.Decrypt(_lionAbpProOptions.Github.Token);
            var release = await _githubManager.GetReleaseVersionUrlAsync(_lionAbpProBasicTemplateOptions.Github.Author, _lionAbpProBasicTemplateOptions.Github.RepositoryName, token, version);

            Logger.LogInformation($"{_lionAbpProBasicTemplateOptions.Github.RepositoryName}版本:{release.TagName}.");
            var outputPath = GetOutputPath(projectName);

            // 源码下载路径
            var downFilePath = Path.Combine(CliPaths.AbpRootPath, _lionAbpProBasicTemplateOptions.Github.RepositoryName, release.TagName) + ".zip";
            Logger.LogInformation($"正在下载{_lionAbpProBasicTemplateOptions.Github.RepositoryName}源码...");
            // 下载源码路径
            await _githubManager.DownloadAsync(release.DownloadUrl, downFilePath);
            Logger.LogInformation($"{_lionAbpProBasicTemplateOptions.Github.RepositoryName}下载完成.");
            // 解压路径
            var targetPath = downFilePath.Replace(".zip", "");
            Logger.LogInformation($"正在解压{_lionAbpProBasicTemplateOptions.Github.RepositoryName}...");
            _zipManager.ExtractZips(downFilePath, targetPath);
            Logger.LogInformation($"{_lionAbpProBasicTemplateOptions.Github.RepositoryName}解压完成.");

            Logger.LogInformation($"正在生成{_lionAbpProBasicTemplateOptions.Github.RepositoryName}...");
            //将解压之后的文件复制到输出目录
            _fileManager.CopyFolder(Path.Combine(targetPath, $"{_lionAbpProBasicTemplateOptions.Github.RepositoryName}-{release.TagName}"), outputPath,
                _lionAbpProBasicTemplateOptions.Replace.ExcludeFiles);
            // 替换文件
            _replaceManager.ReplaceTemplates(outputPath, _lionAbpProBasicTemplateOptions.Replace.OldCompanyName, _lionAbpProBasicTemplateOptions.Replace.OldProjectName, companyName, projectName,
                _lionAbpProBasicTemplateOptions.Replace.ReplaceSuffix);

            Logger.LogInformation($"{_lionAbpProBasicTemplateOptions.Github.RepositoryName}生成成功.");
            Logger.LogInformation($"{_lionAbpProBasicTemplateOptions.Github.RepositoryName}输出路径:{outputPath}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"程序异常：{ex.Message}");
        }
    }

    /// <summary>
    /// 生成Abp-Vnext-Pro-Basic
    /// </summary>
    /// <param name="companyName">公司名称</param>
    /// <param name="projectName">项目名称</param>
    /// <param name="version">版本</param>
    public async Task LionAbpProBasicNoOcelotAsync(
        string companyName,
        string projectName,
        string version)
    {
        try
        {
            Logger.LogInformation($"读取{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}版本信息...");

            var token = Token.Decrypt(_lionAbpProOptions.Github.Token);
            var release = await _githubManager.GetReleaseVersionUrlAsync(_lionAbpProBasicNoOcelotTemplateOptions.Github.Author, _lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName, token, version);

            Logger.LogInformation($"{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}版本:{release.TagName}.");
            var outputPath = GetOutputPath(projectName);

            // 源码下载路径
            var downFilePath = Path.Combine(CliPaths.AbpRootPath, _lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName, release.TagName) + ".zip";
            Logger.LogInformation($"正在下载{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}源码...");
            // 下载源码路径
            await _githubManager.DownloadAsync(release.DownloadUrl, downFilePath);
            Logger.LogInformation($"{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}下载完成.");
            // 解压路径
            var targetPath = downFilePath.Replace(".zip", "");
            Logger.LogInformation($"正在解压{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}...");
            _zipManager.ExtractZips(downFilePath, targetPath);
            Logger.LogInformation($"{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}解压完成.");

            Logger.LogInformation($"正在生成{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}...");
            //将解压之后的文件复制到输出目录
            _fileManager.CopyFolder(Path.Combine(targetPath, $"{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}-{release.TagName}"), outputPath,
                _lionAbpProBasicNoOcelotTemplateOptions.Replace.ExcludeFiles);
            // 替换文件
            _replaceManager.ReplaceTemplates(outputPath, _lionAbpProBasicNoOcelotTemplateOptions.Replace.OldCompanyName, _lionAbpProBasicNoOcelotTemplateOptions.Replace.OldProjectName, companyName,
                projectName,
                _lionAbpProBasicNoOcelotTemplateOptions.Replace.ReplaceSuffix);

            Logger.LogInformation($"{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}生成成功.");
            Logger.LogInformation($"{_lionAbpProBasicNoOcelotTemplateOptions.Github.RepositoryName}输出路径:{outputPath}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"程序异常：{ex.Message}");
        }
    }


    /// <summary>
    /// 生成Abp-Vnext-Pro-Module
    /// </summary>
    /// <param name="companyName">公司名称</param>
    /// <param name="projectName">项目名称</param>
    /// <param name="moduleName">模块名称</param>
    /// <param name="version">版本</param>
    /// <param name="output">输出路径</param>
    public async Task LionAbpProModuleAsync(
        string companyName,
        string projectName,
        string moduleName,
        string version)
    {
        try
        {
            Logger.LogInformation($"读取{_lionAbpProModuleTemplateOptions.Github.RepositoryName}版本信息...");
            var token = Token.Decrypt(_lionAbpProOptions.Github.Token);
            var release = await _githubManager.GetReleaseVersionUrlAsync(_lionAbpProModuleTemplateOptions.Github.Author, _lionAbpProModuleTemplateOptions.Github.RepositoryName, token,
                version);

            Logger.LogInformation($"{_lionAbpProModuleTemplateOptions.Github.RepositoryName}版本:{release.TagName}.");
            var outputPath = GetOutputPath(projectName);

            // 源码下载路径
            var downFilePath = Path.Combine(CliPaths.AbpRootPath, _lionAbpProModuleTemplateOptions.Github.RepositoryName, release.TagName) + ".zip";
            Logger.LogInformation($"正在下载{_lionAbpProModuleTemplateOptions.Github.RepositoryName}源码...");
            // 下载源码路径
            await _githubManager.DownloadAsync(release.DownloadUrl, downFilePath);
            Logger.LogInformation($"{_lionAbpProModuleTemplateOptions.Github.RepositoryName}下载完成.");
            // 解压路径
            var targetPath = downFilePath.Replace(".zip", "");
            Logger.LogInformation($"正在解压{_lionAbpProModuleTemplateOptions.Github.RepositoryName}...");
            _zipManager.ExtractZips(downFilePath, targetPath);
            Logger.LogInformation($"{_lionAbpProModuleTemplateOptions.Github.RepositoryName}解压完成.");

            Logger.LogInformation($"正在生成{_lionAbpProModuleTemplateOptions.Github.RepositoryName}...");
            //将解压之后的文件复制到输出目录
            _fileManager.CopyFolder(Path.Combine(targetPath, $"{_lionAbpProModuleTemplateOptions.Github.RepositoryName}-{release.TagName}"), outputPath,
                _lionAbpProModuleTemplateOptions.Replace.ExcludeFiles);
            // 替换文件
            _replaceManager.ReplaceTemplates(outputPath,
                _lionAbpProModuleTemplateOptions.Replace.OldCompanyName,
                _lionAbpProModuleTemplateOptions.Replace.OldProjectName,
                _lionAbpProModuleTemplateOptions.Replace.OldModuleName,
                companyName,
                projectName,
                moduleName,
                _lionAbpProModuleTemplateOptions.Replace.ReplaceSuffix);

            Logger.LogInformation($"{_lionAbpProModuleTemplateOptions.Github.RepositoryName}生成成功.");
            Logger.LogInformation($"{_lionAbpProModuleTemplateOptions.Github.RepositoryName}输出路径:{outputPath}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"程序异常：{ex.Message}");
        }
    }
}