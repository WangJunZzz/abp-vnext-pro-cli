https://www.cnblogs.com/stulzq/p/9127030.html
dotnet pack
dotnet tool install -g lion.abp --add-source ./
dotnet tool uninstall Lion.AbpPro.Cli -g
dotnet tool update -g lion.abp --add-source ./

dotnet nuget push AppLogger.1.0.0.nupkg --api-key qz2jga8pl3dvn2akksyquwcs9ygggg4exypy3bhxy6w6x6 --source https://api.nuget.org/v3/index.json

lion.abp new abp-vnext-pro -c asdf -p asdf -v 5.3.2.1


dotnet pack -c Release /p:Version=1.2.3