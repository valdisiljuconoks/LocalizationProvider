using System;

namespace DbLocalizationProvider.Tests.KnownAttributesTests;

public class AttributeWithDefaultTranslationAttribute : Attribute
{
    private readonly string _defaultTranslation;

    public AttributeWithDefaultTranslationAttribute(string defaultTranslation)
    {
        _defaultTranslation = defaultTranslation;
    }

    public override string ToString()
    {
        return _defaultTranslation;
    }
}
