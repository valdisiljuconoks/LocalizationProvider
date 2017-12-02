using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.MvcSample.Resources
{
    [LocalizedResource]
    public class CommonResources
    {
        [LocalizedResource]
        public class DialogResources
        {
            public static string YesButton { get; set; }
        }

        [TranslationForCulture("Okk!", "no")]
        public static string Ok => "Ok2";
    }
}
