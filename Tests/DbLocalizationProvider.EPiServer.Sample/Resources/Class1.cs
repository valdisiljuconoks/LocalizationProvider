using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.EPiServer.Sample.Resources
{
    [LocalizedResource]
    public class Class1
    {
        public static string MessageWithFormatArguments => "Here will be value `{0}`";

        [TranslationForCulture("Det är svenska (EDIT)", "sv")]
        public static string ResourceWithSwedishTranslation => "This is English";

        public static string TermsAndConditions => @"I have read the <a href=""{0}"">terms and conditions</a> and agree to them";

        [LocalizedResource]
        public class SomeNestedClass
        {
            public static string Res1 => "This is nested resource";
        }
    }

    [LocalizedModel]
    public class SampleModel
    {
        [Required]
        [StringLength(10)]
        public string Username { get; set; }
    }
}
