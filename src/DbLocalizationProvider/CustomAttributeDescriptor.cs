using System;

namespace DbLocalizationProvider
{
    public class CustomAttributeDescriptor
    {
        public CustomAttributeDescriptor(Type target, bool generateTranslation = true)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if(!typeof(Attribute).IsAssignableFrom(target))
                throw new ArgumentException($"Given type `{target.FullName}` is not of type `System.Attribute`");

            CustomAttribute = target;
            GenerateTranslation = generateTranslation;
        }

        public Type CustomAttribute { get; set; }

        public bool GenerateTranslation { get; private set; }
    }
}
