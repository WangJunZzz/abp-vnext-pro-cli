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
                Token = "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"
            };
            options.Replace = new LionAbpProReplaceOptions()
            {
                OldCompanyName = "Lion",
                OldProjectName = "AbpPro",
                ReplaceSuffix = ".sln,.csproj,.cs,.cshtml,.json,.ci,.yml,.yaml,.nswag,.DotSettings,.env",
                ExcludeFiles = "docs,.github,LICENSE,Readme.md",
            };
        });
    }
}