using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Abstractions.Refactoring;

namespace DbLocalizationProvider.EPiServer.Sample.Resources
{
    [LocalizedResource]
    public class Class1
    {
        [RenamedResource("JustSimpelResource")]
        public static string JustSimpleResource => "Sample resource!";

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

    [LocalizedResource]
    public class NullResource
    {
        public static string NullProperty => "null";
    }

    //[LocalizedResource]
    //public class BadResources
    //{
    //    [TranslationForCulture("Bad translation", "Bad Language Code")]
    //    public static string WrongTranslationLanguage => "Wrong language";
    //}

    [LocalizedResource]
    public class SomeXPathResources
    {
        public static string AnotherRes { get; set; } = "Something weird";

        [ResourceKey("/some/resource/in/xpath1")]
        public static string Xpath1 => "Translation for XPath1";
    }
}
