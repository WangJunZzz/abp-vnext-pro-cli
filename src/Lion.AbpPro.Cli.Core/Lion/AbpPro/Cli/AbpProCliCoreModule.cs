using Lion.AbpPro.Cli.Commands;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Lion.AbpPro.Cli;

[DependsOn(
    typeof(AbpDddDomainModule)
)]
public class AbpProCliCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpCliOptions>(options => { options.Commands[HelpCommand.Name] = typeof(HelpCommand); });
        Configure<AbpCliOptions>(options => { options.Commands[NewCommand.Name] = typeof(NewCommand); });

        context.Services.AddHttpClient();
        context.Services.Configure<LionAbpProOptions>(options =>
        {
            options.Github = new LionAbpProGithubOptions
            {
                Author = "WangJunZzz",
                RepositoryName = "abp-vnext-pro",
                // TODO github   Personal access tokens 经过rsa加密后的密码
                // TODO 为什么要这么做，因为gihub会扫描共有仓库得 Personal access tokens 然后进行比对，删除。
                Token = $"LHgVgvCdHUzqqiUeexqC0Q5GTGzlXEmlI99mhhrFldKqAoq6GJs2gmqmxBUaVWQ4fdx759/bzXLyF/e083035mYesQ/pVnnzv8dMV+EF+RtW/cfpi/GLNr+C6sJGHnxyG4qicph4PqsItP6Xoa504MUwYC/mlUIz3JhY0BYA6wA="
            };
            options.Replace = new LionAbpProReplaceOptions()
            {
                OldCompanyName = "Lion",
                OldProjectName = "AbpPro",
                ReplaceSuffix = ".sln,.csproj,.cs,.cshtml,.json,.ci,.yml,.yaml,.nswag,.DotSettings,.env",
                ExcludeFiles = "docs,.github,LICENSE,Readme.md",
            };
        });
        context.Services.Configure<LionAbpProBasicTemplateOptions>(options =>
        {
            options.Github = new LionAbpProBasicTemplateGithubOptions
            {
                Author = "WangJunZzz",
                RepositoryName = "abp-vnext-pro-templates",
                // TODO github   Personal access tokens 经过rsa加密后的密码
                // TODO 为什么要这么做，因为gihub会扫描共有仓库得 Personal access tokens 然后进行比对，删除。
                Token = $"LHgVgvCdHUzqqiUeexqC0Q5GTGzlXEmlI99mhhrFldKqAoq6GJs2gmqmxBUaVWQ4fdx759/bzXLyF/e083035mYesQ/pVnnzv8dMV+EF+RtW/cfpi/GLNr+C6sJGHnxyG4qicph4PqsItP6Xoa504MUwYC/mlUIz3JhY0BYA6wA="
            };
            options.Replace = new LionAbpProBasicTemplateReplaceOptions()
            {
                OldCompanyName = "MyCompanyName",
                OldProjectName = "MyProjectName",
                ReplaceSuffix = ".sln,.csproj,.cs,.cshtml,.json,.ci,.yml,.yaml,.nswag,.DotSettings,.env",
                ExcludeFiles = "abp-vnext-pro-basic-no-ocelot-nuget,LICENSE,Readme.md",
            };
        });
        context.Services.Configure<LionAbpProBasicNoOcelotTemplateOptions>(options =>
        {
            options.Github = new LionAbpProBasicTemplateGithubOptions()
            {
                Author = "WangJunZzz",
                RepositoryName = "abp-vnext-pro-templates",
                // TODO github   Personal access tokens 经过rsa加密后的密码
                // TODO 为什么要这么做，因为gihub会扫描共有仓库得 Personal access tokens 然后进行比对，删除。
                Token = $"LHgVgvCdHUzqqiUeexqC0Q5GTGzlXEmlI99mhhrFldKqAoq6GJs2gmqmxBUaVWQ4fdx759/bzXLyF/e083035mYesQ/pVnnzv8dMV+EF+RtW/cfpi/GLNr+C6sJGHnxyG4qicph4PqsItP6Xoa504MUwYC/mlUIz3JhY0BYA6wA="
            };
            options.Replace = new LionAbpProBasicTemplateReplaceOptions()
            {
                OldCompanyName = "MyCompanyName",
                OldProjectName = "MyProjectName",
                ReplaceSuffix = ".sln,.csproj,.cs,.cshtml,.json,.ci,.yml,.yaml,.nswag,.DotSettings,.env",
                ExcludeFiles = "abp-vnext-pro-basic-nuget,LICENSE,Readme.md",
            };
        });
    }
}