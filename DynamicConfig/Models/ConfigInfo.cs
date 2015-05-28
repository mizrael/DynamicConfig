using System;

namespace DynamicConfig.Models
{
    public class ConfigInfo
    {
        public ConfigInfo(string uniqueName)
        {
            if (string.IsNullOrWhiteSpace(uniqueName))
                throw new ArgumentNullException("uniqueName");
            this.UniqueName = uniqueName;
        }

        public string UniqueName { get; private set; }

        public string Filename { get; set; }        

        public ConfigObject Data { get; set; }
    }
}
