using System;

namespace DbLocalizationProvider.Abstractions.Refactoring
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class RenamedResourceAttribute : Attribute
    {
        public RenamedResourceAttribute(string oldName)
        {
            if(string.IsNullOrEmpty(oldName))
                throw new ArgumentNullException(nameof(oldName));
            
            OldName = oldName;
        }
        
        public string OldName { get; set; }
        
        public string OldNamespace { get; set; }
    }
}
