using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Lion.AbpPro.Cli.Core;

[DependsOn(typeof(AbpDddDomainModule))]
public class AbpProCliCoreModule : AbpModule
{
}