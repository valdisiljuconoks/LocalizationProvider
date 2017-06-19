using System;

namespace DbLocalizationProvider.Sync
{
    public class ManualResource
    {
        public ManualResource(string key, string translation)
        {
            if(string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            Key = key;
            Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        }

        public string Key { get; private set; }

        public string Translation { get; private set; }
    }
}
