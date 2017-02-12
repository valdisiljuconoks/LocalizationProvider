using System;

namespace DbLocalizationProvider.Sync
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute { }
}
