using System.Text;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Lion.AbpPro.Cli.Replace;

public class ReplaceManager : DomainService
{
    public void ReplaceTemplates(string sourcePath, string oldCompanyName, string oldProjectName, string companyName, string projectName,string replaceSuffix)
    {
        try
        {
            RenameTemplate(sourcePath, oldCompanyName,oldProjectName,companyName, projectName,replaceSuffix);
        }
        catch (Exception)
        {
            throw new UserFriendlyException("生成模板失败");
        }
    }
    
    private void RenameTemplate(string sourcePath, string oldCompanyName, string oldProjectName, string companyName, string projectName,string replaceSuffix)
    {
        RenameAllDirectories(sourcePath,oldCompanyName,oldProjectName, companyName, projectName);
        RenameAllFileNameAndContent(sourcePath, oldCompanyName,oldProjectName,companyName, projectName,replaceSuffix);
    }
    
    private void RenameAllDirectories(string sourcePath,string oldCompanyName, string oldProjectName,  string companyName, string projectName)
    {
        var directories = Directory.GetDirectories(sourcePath);
        foreach (var subDirectory in directories)
        {
            RenameAllDirectories(subDirectory,oldCompanyName,oldProjectName, companyName, projectName);

            var directoryInfo = new DirectoryInfo(subDirectory);
            if (directoryInfo.Name.Contains(oldCompanyName) ||
                directoryInfo.Name.Contains(oldProjectName))
            {
                var oldDirectoryName = directoryInfo.Name;
                var newDirectoryName = oldDirectoryName.CustomReplace(oldCompanyName,oldProjectName,companyName, projectName);

                var newDirectoryPath = Path.Combine(directoryInfo.Parent?.FullName, newDirectoryName);

                if (directoryInfo.FullName != newDirectoryPath)
                {
                    directoryInfo.MoveTo(newDirectoryPath);
                }
            }
        }
    }

    private void RenameAllFileNameAndContent(string sourcePath,string oldCompanyName, string oldProjectName, string companyName, string projectName,string replaceSuffix)
    {
        var list = new DirectoryInfo(sourcePath)
            .GetFiles()
            .Where(f => replaceSuffix.Contains(f.Extension))
            .ToList();

        var encoding = new UTF8Encoding(false);
        foreach (var fileInfo in list)
        {
            // 改文件内容
            var oldContents = File.ReadAllText(fileInfo.FullName, encoding);
            var newContents = oldContents.CustomReplace(oldCompanyName,oldProjectName,companyName, projectName);

            // 文件名包含模板关键字
            if (fileInfo.Name.Contains(oldCompanyName)
                || fileInfo.Name.Contains(oldProjectName))
            {
                var oldFileName = fileInfo.Name;
                var newFileName = oldFileName.CustomReplace(oldCompanyName,oldProjectName,companyName, projectName);

                var newFilePath = Path.Combine(fileInfo.DirectoryName, newFileName);
                // 无变化才重命名
                if (newFilePath != fileInfo.FullName)
                {
                    File.Delete(fileInfo.FullName);
                }

                File.WriteAllText(newFilePath, newContents, encoding);
            }
            else
                File.WriteAllText(fileInfo.FullName, newContents, encoding);
        }

        foreach (var subDirectory in Directory.GetDirectories(sourcePath))
        {
            RenameAllFileNameAndContent(subDirectory,oldCompanyName, oldProjectName,companyName, projectName,replaceSuffix);
        }
    }
}