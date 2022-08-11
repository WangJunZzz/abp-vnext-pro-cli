using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli.NuGet;

public class NuGetService : DomainService
{
    private readonly IHttpClientFactory _clientFactory;

    public NuGetService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    /// <summary>
    /// 从nuget服务获取最新的版本
    /// </summary>
    /// <param name="packageId">nuget包</param>
    /// <returns></returns>
    public async Task<string> GetLatestVersionOrNullAsync(string packageId)
    {
        var versionList = await GetPackageVersionsFromNuGetOrgAsync(packageId);
        if (versionList == null)
        {
            Logger.LogError("无法从nuget.org获取包版本: " + packageId);
        }
        return versionList.OrderByDescending(e => e).FirstOrDefault();
    }

    private async Task<List<string>> GetPackageVersionsFromNuGetOrgAsync(string packageId)
    {
        var url = $"https://api.nuget.org/v3-flatcontainer/{packageId.ToLowerInvariant()}/index.json";
        return await GetPackageVersionListFromUrlAsync(url);
    }

    private async Task<List<string>> GetPackageVersionListFromUrlAsync(string url)
    {
        try
        {
            var client = _clientFactory.CreateClient();

            var response = await client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            if (content.IsNullOrWhiteSpace()) return null;
            return JsonConvert.DeserializeObject<NuGetVersionResultDto>(content).Versions;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public class NuGetVersionResultDto
{
    [JsonProperty("versions")] public List<string> Versions { get; set; }
}