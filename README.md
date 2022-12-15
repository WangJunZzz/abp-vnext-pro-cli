## 安装
```charp
dotnet tool install Lion.AbpPro.Cli -g
```
## 卸载
```charp
dotnet tool uninstall Lion.AbpPro.Cli -g
```

## 更新
```charp
dotnet tool update Lion.AbpPro.Cli -g
```


## 生成Lion.AbpPro模板
```charp
// abp-vnext-pro 生成源码版本
lion.abp new abp-vnext-pro -c 公司名称 -p 项目名称  -v 版本号(可选)
// abp-vnext-pro-basic nuget包形式的基础版本，包裹abp自带的所有模块，已经pro的通知模块，数据字典模块 以及ocelot网关
lion.abp new abp-vnext-pro-basic -c 公司名称 -p 项目名称 -v 版本(默认LastRelease) 
// abp-vnext-pro-basic nuget包形式的基础版本，包裹abp自带的所有模块，已经pro的通知模块，数据字典模块 无ocelot网关
lion.abp new abp-vnext-pro-basic-no-ocelot -c 公司名称 -p 项目名称 -v 版本(默认LastRelease) 

```
