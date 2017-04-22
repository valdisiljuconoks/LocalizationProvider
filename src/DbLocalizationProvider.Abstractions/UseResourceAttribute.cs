using System;

namespace DbLocalizationProvider.Abstractions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UseResourceAttribute : Attribute
    {
        public UseResourceAttribute(Type targetContainer, string propertyName)
        {
            TargetContainer = targetContainer ?? throw new ArgumentNullException(nameof(targetContainer));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public Type TargetContainer { get; }

        public string PropertyName { get; }
    }
}
