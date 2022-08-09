using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli.Zip;

public class ZipManager : DomainService
{
    private readonly ILogger<ZipManager> _logger;

    public ZipManager(ILogger<ZipManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 解压zip文件
    /// </summary>
    /// <param name="sourcePath">解压文件地址</param>
    /// <param name="targetPath">解压文件</param>
    /// <returns></returns>
    public void ExtractZips(string sourcePath, string targetPath)
    {
        Check.NotNullOrWhiteSpace(sourcePath, nameof(sourcePath));
        Check.NotNullOrWhiteSpace(targetPath, nameof(targetPath));

        if (!File.Exists(sourcePath)) throw new UserFriendlyException("解压文件不存在");


        var fileName = Path.GetFileName(sourcePath);
        if (fileName.IsNullOrWhiteSpace()) throw new UserFriendlyException("文件路径不正确");

        if (Directory.Exists(targetPath)) return;
        if (!File.Exists(targetPath))
        {
            ZipFile.ExtractToDirectory(sourcePath, targetPath);
        }
    }
}