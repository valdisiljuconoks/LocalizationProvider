using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;

namespace DbLocalizationProvider.EPiServer.Sample.Models
{
    public class SampleSelectionFactory : ISelectionFactory
    {
        public static List<ISelectItem> _values = new List<ISelectItem>();

        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            return _values;
        }

        public static void AddNewValue(string key, string val)
        {
            _values.Add(new SelectItem { Value = key, Text = val });
        }
    }
}
