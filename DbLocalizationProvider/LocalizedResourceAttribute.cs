using System;

namespace DbLocalizationProvider
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class LocalizedResourceAttribute : Attribute { }
}
