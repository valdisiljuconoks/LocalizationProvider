using System;

namespace DbLocalizationProvider
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
    public class HiddenAttribute : Attribute { }
}
