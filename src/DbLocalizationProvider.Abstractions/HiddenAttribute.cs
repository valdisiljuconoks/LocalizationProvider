using System;

namespace DbLocalizationProvider.Abstractions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
    public class HiddenAttribute : Attribute { }
}
