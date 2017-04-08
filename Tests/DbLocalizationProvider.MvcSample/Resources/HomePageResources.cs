namespace DbLocalizationProvider.MvcSample.Resources
{
    [LocalizedResource]
    public class HomePageResources
    {
        public static string Header => "This is home page header!";

        public static string RequiredFieldIndicator => "*";

        public static string SameFieldResource = "This is going to be constant!";

        [TableHeaderTitle]
        public static string WithCustomAttribute => "With custom attribute!";
    }

    [LocalizedResource]
    public class CommonResources
    {
        [LocalizedResource]
        public class DialogResources
        {
            public static string YesButton { get; set; }
        }
    }
}
