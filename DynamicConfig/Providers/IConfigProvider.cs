using System;
using System.Dynamic;

namespace DynamicConfig.Providers
{
    public interface IConfigProvider 
    {
        Models.ConfigObject Load(string uniqueName, string fullPath);
        Models.ConfigObject Parse(string uniqueName, string json);
    }
}
