using System;
using System.Dynamic;

namespace DynamicConfig.Providers
{
    public interface IConfigProvider 
    {
        DynamicConfig.Models.ConfigObject Load(string uniqueName, string fullPath);
        DynamicConfig.Models.ConfigObject Parse(string uniqueName, string json);
    }
}
