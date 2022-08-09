﻿namespace Lion.AbpPro.Cli.Replace;

public static class ReplaceExtension
{
    public static string CustomReplace(this string content,string oldCompanyName, string oldProjectName, string companyName,string projectName)
    {
        var result = content
                .Replace(oldCompanyName, companyName)
                .Replace(oldProjectName, projectName)
            ;

        return result;
    }
}