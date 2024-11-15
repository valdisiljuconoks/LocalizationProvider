using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.EPiServer.Sample.Resources
{
    public static class Components
    {
        [LocalizedResource]
        public static class Navigation
        {
            [TranslationForCulture("Meny", "no")]
            [TranslationForCulture("Meny", "sv")]
            public static string Toggle => "Menu";
        }

        [LocalizedResource]
        public static class Shared
        {
            [TranslationForCulture("Handlevogn", "no")]
            [TranslationForCulture("Kundvagn", "sv")]
            public static string Cart => "Cart";

            [TranslationForCulture("Lukk", "no")]
            [TranslationForCulture("Stäng", "sv")]
            public static string Close => "Close";

            [TranslationForCulture("Søk", "no")]
            [TranslationForCulture("Sök", "sv")]
            public static string Search => "Search";

            [TranslationForCulture("Hva søker du etter?", "no")]
            [TranslationForCulture("Vad letar du efter?", "sv")]
            public static string SearchPlaceholder => "What are you looking for?";

            [TranslationForCulture(",-", "no")]
            [TranslationForCulture(",-", "sv")]
            [TranslationForCulture(",-", "dk")]
            [TranslationForCulture(",-", "fi")]
            public static string CurrencySymbol => "$";
        }
    }

    [LocalizedResource]
    public static class Order
    {
        [TranslationForCulture("Bestill nå", "no")]
        [TranslationForCulture("Beställ nu", "sv")]
        public static string OrderNow => "Order now";

        [TranslationForCulture("Shop videre", "no")]
        [TranslationForCulture("Fortsätt handla", "sv")]
        public static string ContinueShopping => "Continue shopping";

        [LocalizedResource]
        public static class Errors
        {
            [TranslationForCulture("Vi beklager men rabattkoden ble ikke funnet.", "no")]
            [TranslationForCulture("Vi beklagar men rabattkoden hittades inte.", "sv")]
            public static string CampaignCodeNotFound => "Campaign code was not found.";
        }
    }

    public static class Views
    {
        [LocalizedResource]
        public static class Errors
        {
            [TranslationForCulture("En feil har skjedd", "no")]
            public static string ErrorHasOccured => "An error has occured";

            [TranslationForCulture("Kontrollernavn", "no")]
            public static string ControllerName => "Controller name";

            [TranslationForCulture("Handlingsnavn", "no")]
            public static string ActionName => "Action name";

            [TranslationForCulture("Stabel spor", "no")]
            public static string Stacktrace => "Stack trace";

            [TranslationForCulture("Vennligst prøv igjen om en liten stund", "no")]
            public static string TryAgain => "Please try again in a few moments";

            [TranslationForCulture("404 - Siden ble ikke funnet", "no")]
            public static string PageNotFound => "404 - Page not found";

            [TranslationForCulture("Vi beklager men siden er enten fjernet eller flyttet.", "no")]
            public static string PageNotFoundText => "We're sorry but the page has either been moved or deleted.";

            [LocalizedResource]
            public static class Rendering
            {
                [TranslationForCulture("En feil under rendring {0} {1}", "no")]
                public static string Heading => "Error while rendering {0} {1}";
            }
        }
    }
}
