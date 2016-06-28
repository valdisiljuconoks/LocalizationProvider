using System;

namespace DbLocalizationProvider
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class ResourceKeyAttribute : Attribute
    {
        public ResourceKeyAttribute(string key)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            Key = key;
        }

        public string Key { get; private set; }

        public string Value { get; set; }
    }
}
