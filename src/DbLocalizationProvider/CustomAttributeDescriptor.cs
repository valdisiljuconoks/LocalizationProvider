using System;
using System.Collections.Generic;

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

        public bool GenerateTranslation { get; }
    }

    public static class CustomAttributeDescriptorCollectionExtensions
    {
        public static ICollection<CustomAttributeDescriptor> Add(this ICollection<CustomAttributeDescriptor> target, Type customAttribute)
        {
            target.Add(new CustomAttributeDescriptor(customAttribute));
            return target;
        }

        public static ICollection<CustomAttributeDescriptor> Add<T>(this ICollection<CustomAttributeDescriptor> target)
        {
            target.Add(new CustomAttributeDescriptor(typeof(T)));
            return target;
        }
    }
}
