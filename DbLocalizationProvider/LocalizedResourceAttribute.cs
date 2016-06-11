using System;

namespace DbLocalizationProvider
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class LocalizedResourceAttribute : Attribute
    {
        public string KeyPrefix { get; set; }
    }
}
