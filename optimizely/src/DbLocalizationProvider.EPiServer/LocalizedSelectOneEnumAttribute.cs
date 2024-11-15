using System;
using EPiServer.Shell.ObjectEditing;

namespace DbLocalizationProvider.EPiServer;

/// <inheritdoc />
public class LocalizedSelectOneEnumAttribute : SelectOneAttribute
{
    private readonly Type _enumType;

    /// <inheritdoc />
    public LocalizedSelectOneEnumAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException($"Type of the argument '{enumType}' is not 'System.Enum'.", nameof(enumType));
        }

        _enumType = enumType;
    }

    /// <inheritdoc />
    public override Type SelectionFactoryType
    {
        get
        {
            return typeof(LocalizedEnumSelectionFactory<>).MakeGenericType(_enumType);
        }
        set
        {
            base.SelectionFactoryType = value;
        }
    }
}
