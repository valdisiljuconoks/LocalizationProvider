using System;
using System.Collections.Generic;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    public class ForeignResourceDescriptor
    {
        public ForeignResourceDescriptor(Type target)
        {
            ResourceType = target ?? throw new ArgumentNullException(nameof(target));
        }

        public Type ResourceType { get; }
    }

    public static class ICollectionOfForeignResourceDescriptorExtensions
    {
        public static void Add(this ICollection<ForeignResourceDescriptor> collection, Type target)
        {
            collection.Add(new ForeignResourceDescriptor(target));
        }

        public static void AddRange(this ICollection<ForeignResourceDescriptor> collection, IEnumerable<Type> targets)
        {
            targets.ForEach(t => collection.Add(new ForeignResourceDescriptor(t)));
        }
    }
}
