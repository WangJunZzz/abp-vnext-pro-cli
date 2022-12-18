using System.Text;
using Lion.AbpPro.Cli.Args;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;

namespace Lion.AbpPro.Cli.Commands;

public class NewCommand : IConsoleCommand, ITransientDependency
{
    public const string Name = "new";
    private readonly ILogger<NewCommand> _logger;
    private readonly AbpCliOptions _abpCliOptions;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly GenerateCodeManager _generateCodeManager;

    public NewCommand(IOptions<AbpCliOptions> abpCliOptions, ILogger<NewCommand> logger, IServiceScopeFactory serviceScopeFactory, GenerateCodeManager generateCodeManager)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _generateCodeManager = generateCodeManager;
        _abpCliOptions = abpCliOptions.Value;
    }

    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Target.IsNullOrWhiteSpace())
        {
            GetUsageInfo();
            return;
        }

        //校验是否输入公司名称
        var company = commandLineArgs.Options.GetOrNull(CommandOptions.Company.Short, CommandOptions.Company.Long);
        if (company.IsNullOrWhiteSpace())
        {
            _logger.LogError("请输入公司名称");
            return;
        }

        //校验是否输入项目名称
        var project = commandLineArgs.Options.GetOrNull(CommandOptions.Project.Short, CommandOptions.Project.Long);
        if (project.IsNullOrWhiteSpace())
        {
            _logger.LogError("请输入项目名称");
            return;
        }

        //版本
        var version = commandLineArgs.Options.GetOrNull(CommandOptions.Version.Short, CommandOptions.Version.Long);

        // 输出目录
        var output = commandLineArgs.Options.GetOrNull(CommandOptions.OutputFolder.Short, CommandOptions.OutputFolder.Long);

        if (commandLineArgs.Target == LionAbpProCliConsts.LionAbpPro)
        {
            await _generateCodeManager.LionAbpProAsync(company, project, version);
        }
        else if (commandLineArgs.Target == LionAbpProCliConsts.LionAbpProBasic)
        {
            await _generateCodeManager.LionAbpProBasicAsync(company, project, version);
        }
        else if (commandLineArgs.Target == LionAbpProCliConsts.LionAbpProBasicNoOcelot)
        {
            await _generateCodeManager.LionAbpProBasicNoOcelotAsync(company, project, version);
        }
        else if (commandLineArgs.Target == LionAbpProCliConsts.LionAbpProModule)
        {
            //校验是否输入模块名称
            var module = commandLineArgs.Options.GetOrNull(CommandOptions.Module.Short, CommandOptions.Module.Long);
            if (module.IsNullOrWhiteSpace())
            {
                _logger.LogError("请输入模块名称");
                return;
            }

            await _generateCodeManager.LionAbpProModuleAsync(company, project, module, version);
        }
        else
        {
            _logger.LogError($"{commandLineArgs.Target}模板类型不存在");
        }
    }

    public void GetUsageInfo()
    {
        var sb = new StringBuilder();

        sb.AppendLine("查看命令帮助:");
        sb.AppendLine("    lion.abp help");
        sb.AppendLine("命令列表:");

        foreach (var command in _abpCliOptions.Commands.ToArray())
        {
            string shortDescription;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                shortDescription = ((IConsoleCommand)scope.ServiceProvider
                    .GetRequiredService(command.Value)).GetShortDescription();
            }

            sb.Append("    > ");
            sb.Append(command.Key);
            sb.Append(string.IsNullOrWhiteSpace(shortDescription) ? "" : ":");
            sb.Append(" ");
            sb.AppendLine(shortDescription);
        }

        _logger.LogInformation(sb.ToString());
    }

    public string GetShortDescription()
    {
        var message = Environment.NewLine;
        message += $"           > lion.abp new abp-vnext-pro -c 公司名称 -p 项目名称 -v 版本(默认LastRelease)";
        message += Environment.NewLine;
        message += $"           > lion.abp new abp-vnext-pro-basic -c 公司名称 -p 项目名称 -v 版本(默认LastRelease)";
        message += Environment.NewLine;
        message += $"           > lion.abp new abp-vnext-pro-basic-no-ocelot -c 公司名称 -p 项目名称 -v 版本(默认LastRelease)";
        message += Environment.NewLine;
        message += $"           > lion.abp new abp-vnext-pro-module -c 公司名称 -p 项目名称 -m 模块名称 -v 版本(默认LastRelease)";
        return message;
    }
}