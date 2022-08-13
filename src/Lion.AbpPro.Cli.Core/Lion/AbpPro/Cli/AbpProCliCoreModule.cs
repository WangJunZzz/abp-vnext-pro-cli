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
                // TODO 加上前缀，因为github会自动检测person_token,检测到了会自动删除。
                Token = $"{LionAbpProCliConsts.LionAbpPro}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"
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