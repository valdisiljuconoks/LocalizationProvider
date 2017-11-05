using System.Globalization;

namespace DbLocalizationProvider.Commands
{
    public class CreateOrUpdateTranslation
    {
        public class Command : ICommand
        {
            public Command(string key, CultureInfo language, string translation)
            {
                Key = key;
                Language = language;
                Translation = translation;
            }

            public string Key { get; }

            public CultureInfo Language { get; }

            public string Translation { get; }
        }
    }
}
