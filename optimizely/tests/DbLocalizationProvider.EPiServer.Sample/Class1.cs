using DbLocalizationProvider;
using DbLocalizationProvider.Abstractions;

namespace MyProject
{
    [LocalizedResource]
    public class MyResources
    {
        public static string FirstProperty => "One";
        public static string SecondProperty => "Two (EDIT)";
        public static string FifthProperty => "Fifth";

        public string InstanceProperty = "Instance property";

        [TranslationForCulture("English", "en")]
        [TranslationForCulture("", "no")]
        public static string EmptyInNorwegian => "Empty in Norwegian";
    }

    [LocalizedResource]
    public class AlternativeResources
    {
        public static string ThirdProperty => "Three";
        public static string ForthProperty => "Four (EDIT)";
    }
}
