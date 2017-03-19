using DbLocalizationProvider;

namespace MyProject
{
    [LocalizedResource]
    public class MyResources
    {
        public static string FirstProperty => "One";
        public static string SecondProperty => "Two";
    }

    [LocalizedResource]
    public class AlternativeResources
    {
        public static string ThirdProperty => "Three";
        public static string ForthProperty => "Four";
    }
}
