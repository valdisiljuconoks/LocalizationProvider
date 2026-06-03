using DbLocalizationProvider.Abstractions;

namespace AlloySampleSite.Resources
{
    [LocalizedResource(KeyPrefix = "Custom")]
    public class ParentLabel
    {
        public static MyChildLabels Child { get; }

        [LocalizedResource]
        public class MyChildLabels
        {
            public string Label1 => "Label 1";

            public string Label2 => "Label 2";
        }
    }
}
