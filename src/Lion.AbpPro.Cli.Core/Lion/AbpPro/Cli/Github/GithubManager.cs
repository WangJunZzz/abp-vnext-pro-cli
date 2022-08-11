using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli.Github;

public class GithubManager : DomainService
{
    private readonly IConfiguration _configuration;

    public GithubManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 获取指定仓库最后一次Release版本地址
    /// </summary>
    /// <param name="author">作者</param>
    /// <param name="repositoryName">仓库名称</param>
    /// <param name="token">github 访问令牌</param> 
    /// <param name="version">版本</param>
    /// <returns>Uri</returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<GithubReleaseResult> GetReleaseVersionUrlAsync(string author,
        string repositoryName,
        string token,
        string version = "")
    {
        try
        {
            Check.NotNullOrWhiteSpace(author, nameof(author));
            Check.NotNullOrWhiteSpace(repositoryName, nameof(author));
            var github = new GitHubClient(new ProductHeaderValue(repositoryName))
            {
                // 匿名访问，api会限流，所以需要设置访问令牌
                Credentials = new Credentials(token)
            };
            Release release = null;
            if (version.IsNullOrWhiteSpace())
            {
                release = await github.Repository.Release.GetLatest(author, repositoryName);
            }
            else
            {
                release = await github.Repository.Release.Get(author, repositoryName, version);
            }

            if (release != null)
            {
                var result = new GithubReleaseResult();
                Logger.LogInformation($"{repositoryName}：{release.TagName}");
                result.DownloadUrl = new Uri($"https://github.com/{author}/{repositoryName}/archive/refs/tags/{release.TagName}.zip");
                result.TagName = release.TagName;
                return result;
            }
            else
            {
                throw new UserFriendlyException("没有找到最新版本.");
            }
        }
        catch (NotFoundException)
        {
            Logger.LogError($"版本不存在.");
        }
        catch (RateLimitExceededException)
        {
            Logger.LogError($"访问Github API超过了限制，请稍后再试.");
        }
        catch (Exception)
        {
            Logger.LogError($"获取{repositoryName}失败.");
            throw;
        }

        return null;
    }

    /// <summary>
    /// 下载源码
    /// </summary>
    /// <param name="uri">下载地址</param>
    /// <param name="path">下载保存路径</param>
    public async Task DownloadAsync(Uri uri,
        string path)
    {
        try
        {
            if (File.Exists(path)) return;
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var pathDirectory = Path.GetDirectoryName(path);
                if (!Directory.Exists(pathDirectory))
                {
                    Directory.CreateDirectory(pathDirectory);
                }

                using (FileStream fileStream = new FileStream(path, System.IO.FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError("下载源码失败：" + e.Message);
        }
    }
}

public class GithubReleaseResult
{
    public Uri DownloadUrl { get; set; }
    
    public string TagName { get; set; }
}