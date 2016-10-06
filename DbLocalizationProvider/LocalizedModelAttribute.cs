using System;

namespace DbLocalizationProvider
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LocalizedModelAttribute : Attribute
    {
        public string KeyPrefix { get; set; }

        public bool Inherited { get; set; } = true;
    }
}
