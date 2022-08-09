using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli.Zip;

public class FileManager : DomainService
{
    /// <summary>
    /// 把一个文件夹下所有文件复制到另一个文件夹下 
    /// </summary>
    /// <param name="sourcePath">源目录</param>
    /// <param name="destPath">目标目录</param>
    public void Copy(string sourcePath, string destPath)
    {
        try
        {
            string floderName = Path.GetFileName(sourcePath);
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(destPath, floderName));
            string[] files = Directory.GetFileSystemEntries(sourcePath);

            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    Copy(file, di.FullName);
                }
                else
                {
                    File.Copy(file, Path.Combine(di.FullName, Path.GetFileName(file)), true);
                }
            }
        }
        catch (Exception)
        {
            throw new UserFriendlyException("复制文件失败！");
        }
    }
}