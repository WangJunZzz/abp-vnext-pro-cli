using Volo.Abp;

namespace Lion.AbpPro.Cli.Cryptography;

public static class Token
{
    /// <summary> 
    /// 解密数据 
    /// </summary> 
    /// <param name="data">要解密数据</param> 
    /// <param name="keyContainerName">密匙容器的名称</param> 
    /// <returns></returns> 
    public static string Decrypt(string data, string keyContainerName = "abp-vnext-pro")
    {
        Check.NotNullOrWhiteSpace(data, nameof(data));
        return data.Replace(keyContainerName, "");
    }
}