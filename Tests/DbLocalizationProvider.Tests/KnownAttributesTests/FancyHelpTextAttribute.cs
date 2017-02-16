using System;

namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class FancyHelpTextAttribute : Attribute { }
}