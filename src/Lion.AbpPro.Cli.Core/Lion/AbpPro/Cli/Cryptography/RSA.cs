using System.Diagnostics.CodeAnalysis;

namespace Lion.AbpPro.Cli.Cryptography;

[SuppressMessage("Interoperability", "CA1416:验证平台兼容性")]
public static class RSA
{
    /// <summary> 
    /// RSA加密数据 
    /// </summary> 
    /// <param name="data">要加密数据</param> 
    /// <param name="keyContainerName">密匙容器的名称</param> 
    /// <returns></returns> 
    public static string Encryption(string data, string keyContainerName)
    {

        System.Security.Cryptography.CspParameters param = new System.Security.Cryptography.CspParameters();
        param.KeyContainerName = keyContainerName ; //密匙容器的名称，保持加密解密一致才能解密成功
        using (System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(param))
        {
            byte[] plaindata = System.Text.Encoding.Default.GetBytes(data);//将要加密的字符串转换为字节数组
            byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
            return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
        }
    }
    /// <summary> 
    /// RSA解密数据 
    /// </summary> 
    /// <param name="data">要解密数据</param> 
    /// <param name="keyContainerName">密匙容器的名称</param> 
    /// <returns></returns> 
    public static string Decrypt(string data, string keyContainerName)
    {
        System.Security.Cryptography.CspParameters param = new System.Security.Cryptography.CspParameters();
        param.KeyContainerName =keyContainerName; //密匙容器的名称，保持加密解密一致才能解密成功
        using (System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(param))
        {
            byte[] encryptdata = Convert.FromBase64String(data);
            byte[] decryptdata = rsa.Decrypt(encryptdata, false);
            return System.Text.Encoding.Default.GetString(decryptdata);
        }
    }
}