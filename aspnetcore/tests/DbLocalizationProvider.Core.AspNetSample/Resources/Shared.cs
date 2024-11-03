using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Core.AspNetSample.Resources;

public class Shared
{
    [LocalizedResource]
    public class CommonResources
    {
        public static string Yes => "Yup";

        public string No => "Nop";
    }
}
