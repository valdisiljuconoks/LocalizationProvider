using System;

namespace DbLocalizationProvider.Core.AspNetSample.Resources;

public class WeirdCustomAttribute : Attribute
{
    public WeirdCustomAttribute(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }
}
