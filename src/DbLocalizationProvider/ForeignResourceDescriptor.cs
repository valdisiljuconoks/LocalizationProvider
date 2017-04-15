using System;

namespace DbLocalizationProvider
{
    public class ForeignResourceDescriptor
    {
        public ForeignResourceDescriptor(Type target)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            ResourceType = target;
        }

        public Type ResourceType { get; }
    }
}
